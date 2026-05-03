using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Attempts.Queries;

public record GetExamResultQuery(Guid AttemptId, Guid UserId) : IRequest<ExamResultDto>;

public class GetExamResultQueryHandler : IRequestHandler<GetExamResultQuery, ExamResultDto>
{
    private readonly IExamPortalDbContext _context;

    public GetExamResultQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<ExamResultDto> Handle(GetExamResultQuery request, CancellationToken cancellationToken)
    {
        // Fetch the attempt and include the ExamSet to get the Title and PassingScore
        var attempt = await _context.ExamAttempts
           .Include(a => a.ExamSet)
           .AsNoTracking()
           .FirstOrDefaultAsync(a => a.Id == request.AttemptId && a.UserId == request.UserId, cancellationToken);

        if (attempt == null || attempt.EndTime == null)
            throw new InvalidOperationException("Result is not available or the exam is not yet submitted.");

        bool passed = attempt.TotalScore >= attempt.ExamSet.PassingScore;

        return new ExamResultDto(
            attempt.Id,
            attempt.ExamSet.Title,
            attempt.TotalScore ?? 0,
            attempt.ExamSet.PassingScore,
            passed,
            attempt.StartTime,
            attempt.EndTime
        );
    }
}