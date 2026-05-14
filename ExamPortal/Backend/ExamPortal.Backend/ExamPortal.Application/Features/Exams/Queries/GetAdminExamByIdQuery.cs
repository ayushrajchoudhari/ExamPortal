using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Queries;

public record GetAdminExamByIdQuery(Guid ExamId, Guid AdminId) : IRequest<AdminExamDetailsDto>;

public class GetAdminExamByIdQueryHandler : IRequestHandler<GetAdminExamByIdQuery, AdminExamDetailsDto>
{
    private readonly IExamPortalDbContext _context;

    public GetAdminExamByIdQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<AdminExamDetailsDto> Handle(GetAdminExamByIdQuery request, CancellationToken cancellationToken)
    {
        var exam = await _context.ExamSets
           .Include(e => e.InstructionSet)
           .Include(e => e.QuestionSets)
               .ThenInclude(q => q.AnswerOptions)
           .AsNoTracking()
           .FirstOrDefaultAsync(e => e.Id == request.ExamId && e.CreatedByUserId == request.AdminId, cancellationToken);

        if (exam == null)
            throw new UnauthorizedAccessException("Exam not found or you do not have permission.");

        return new AdminExamDetailsDto(
            exam.Id,
            exam.Title,
            exam.DurationMinutes,
            exam.PassingScore,
            exam.IsPublic,
            exam.SecretToken,
            exam.InstructionSet?.Content ?? "",
            exam.QuestionSets.OrderBy(q => q.DisplayOrder).Select(q => new AdminQuestionDto(
                q.Id,
                q.QuestionText,
                q.Points,
                q.DisplayOrder,
                q.AnswerOptions.Select(o => new AdminAnswerOptionDto(o.Id, o.OptionText, o.IsCorrect)).ToList()
            )).ToList()
        );
    }
}