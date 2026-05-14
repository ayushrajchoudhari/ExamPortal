using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Commands;

public record DeleteExamCommand(Guid ExamId, Guid AdminId) : IRequest<bool>;

public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, bool>
{
    private readonly IExamPortalDbContext _context;

    public DeleteExamCommandHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
    {
        // Explicitly check that the logged-in admin is the creator of this exam
        var exam = await _context.ExamSets
           .FirstOrDefaultAsync(e => e.Id == request.ExamId && e.CreatedByUserId == request.AdminId, cancellationToken);

        if (exam == null)
        {
            throw new UnauthorizedAccessException("Exam not found or you do not have permission to delete it.");
        }

        _context.ExamSets.Remove(exam);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}