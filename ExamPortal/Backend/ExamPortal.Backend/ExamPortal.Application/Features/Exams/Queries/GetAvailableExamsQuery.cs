using ExamPortal.Application.DTOs.User; // Assuming this is where ExamSetSummaryDto lives
using ExamPortal.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace ExamPortal.Application.Features.Exams.Queries;

// 1. The Query Request (The payload from the controller)
public record GetAvailableExamsQuery() : IRequest<List<ExamSetSummaryDto>>;

// 2. The Handler (The business logic)
public class GetAvailableExamsQueryHandler : IRequestHandler<GetAvailableExamsQuery, List<ExamSetSummaryDto>>
{
    private readonly IExamPortalDbContext _context;

    // Inject the interface, NOT the SQL context directly
    public GetAvailableExamsQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExamSetSummaryDto>> Handle(GetAvailableExamsQuery request, CancellationToken cancellationToken)
    {
        // High-performance LINQ projection directly to DTO
        return await _context.ExamSets
           .AsNoTracking()
           .Where(e => e.IsActive)
           .Select(e => new ExamSetSummaryDto(
                e.Id,
                e.Title,
                e.DurationMinutes,
                e.PassingScore))
           .ToListAsync(cancellationToken);
    }
}