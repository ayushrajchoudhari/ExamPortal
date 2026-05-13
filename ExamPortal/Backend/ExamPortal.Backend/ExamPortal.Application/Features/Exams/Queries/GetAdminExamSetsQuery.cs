using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Queries;

public record GetAdminExamSetsQuery(Guid AdminId) : IRequest<List<AdminExamSetSummaryDto>>;

public class GetAdminExamSetsQueryHandler : IRequestHandler<GetAdminExamSetsQuery, List<AdminExamSetSummaryDto>>
{
    private readonly IExamPortalDbContext _context;

    public GetAdminExamSetsQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminExamSetSummaryDto>> Handle(GetAdminExamSetsQuery request, CancellationToken cancellationToken)
    {
        return await _context.ExamSets
         .AsNoTracking()
         .Where(e => e.CreatedByUserId == request.AdminId)
         .Select(e => new AdminExamSetSummaryDto(
                e.Id,
                e.Title,
                e.DurationMinutes,
                e.PassingScore,
                e.IsPublic,
                e.QuestionSets.Count,
                e.SecretToken
            ))
         .ToListAsync(cancellationToken);
    }
}