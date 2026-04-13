using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Attempts.Queries;

public record GetActiveExamQuestionsQuery(Guid AttemptId, Guid UserId) : IRequest<List<ActiveQuestionDto>>;

public class GetActiveExamQuestionsQueryHandler : IRequestHandler<GetActiveExamQuestionsQuery, List<ActiveQuestionDto>>
{
    private readonly IExamPortalDbContext _context;

    public GetActiveExamQuestionsQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActiveQuestionDto>> Handle(GetActiveExamQuestionsQuery request, CancellationToken cancellationToken)
    {
        // 1. Verify the attempt belongs to the logged-in user and is still active
        var attempt = await _context.ExamAttempts
           .AsNoTracking()
           .FirstOrDefaultAsync(a => a.Id == request.AttemptId && a.UserId == request.UserId, cancellationToken);

        if (attempt == null || attempt.EndTime != null)
            throw new UnauthorizedAccessException("Attempt not found, unauthorized, or already submitted.");

        // 2. Fetch questions and project them to DTOs (Stripping out the correct answers)
        var questions = await _context.QuestionSets
           .AsNoTracking()
           .Where(q => q.ExamSetId == attempt.ExamSetId)
           .OrderBy(q => q.DisplayOrder)
           .Select(q => new ActiveQuestionDto(
                q.Id,
                q.QuestionText,
                q.Points,
                q.AnswerOptions.Select(o => new ActiveAnswerOptionDto(o.Id, o.OptionText)).ToList()
            ))
           .ToListAsync(cancellationToken);

        return questions;
    }
}