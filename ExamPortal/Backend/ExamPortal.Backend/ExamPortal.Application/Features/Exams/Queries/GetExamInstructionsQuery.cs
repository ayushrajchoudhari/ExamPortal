using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Queries;

public record GetExamInstructionsQuery(Guid ExamSetId) : IRequest<InstructionDto>;

public class GetExamInstructionsQueryHandler : IRequestHandler<GetExamInstructionsQuery, InstructionDto>
{
    private readonly IExamPortalDbContext _context;

    public GetExamInstructionsQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<InstructionDto> Handle(GetExamInstructionsQuery request, CancellationToken cancellationToken)
    {
        var instruction = await _context.InstructionSets
           .AsNoTracking()
           .FirstOrDefaultAsync(i => i.ExamSetId == request.ExamSetId, cancellationToken);

        if (instruction == null)
            throw new Exception("Instructions not found for this exam.");

        return new InstructionDto(instruction.Id, instruction.Content, instruction.AgreementRequired);
    }
}