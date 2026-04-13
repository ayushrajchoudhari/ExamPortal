using MediatR;
using Microsoft.EntityFrameworkCore;
using ExamPortal.Application.Interfaces;
using ExamPortal.Application.DTOs.User;

namespace ExamPortal.Application.Features.Exams.Queries;

public record GetExamInstructionsQuery(Guid ExamId) : IRequest<InstructionDto>;

public class GetExamInstructionsQueryHandler : IRequestHandler<GetExamInstructionsQuery, InstructionDto>
{
    private readonly IExamPortalDbContext _context;

    public GetExamInstructionsQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<InstructionDto> Handle(GetExamInstructionsQuery request, CancellationToken cancellationToken)
    {
        var instruction = await _context.ExamSets
           .AsNoTracking()
           .Where(e => e.Id == request.ExamId)
           .Select(e => e.InstructionSet)
           .FirstOrDefaultAsync(cancellationToken);

        if (instruction == null) throw new Exception("Instructions not found.");

        return new InstructionDto(instruction.Id, instruction.Content, instruction.AgreementRequired);
    }
}