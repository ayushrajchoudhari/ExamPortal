using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using ExamPortal.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Attempts.Commands;

public record SubmitExamCommand(Guid AttemptId, Guid UserId, SubmitExamRequestDto Payload) : IRequest<SubmitExamResponseDto>;

public class SubmitExamCommandHandler : IRequestHandler<SubmitExamCommand, SubmitExamResponseDto>
{
    private readonly IExamPortalDbContext _context;

    public SubmitExamCommandHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<SubmitExamResponseDto> Handle(SubmitExamCommand request, CancellationToken cancellationToken)
    {
        // 1. Load the attempt (Tracking enabled because we will update it)
        var attempt = await _context.ExamAttempts
           .Include(a => a.ExamSet)
           .FirstOrDefaultAsync(a => a.Id == request.AttemptId && a.UserId == request.UserId, cancellationToken);

        if (attempt == null || attempt.EndTime != null)
            throw new InvalidOperationException("Attempt is invalid or has already been submitted.");

        // 2. Fetch all correct answers for this exam into a quick lookup dictionary
        var correctAnswers = await _context.AnswerOptions
           .AsNoTracking()
           .Where(o => o.QuestionSet.ExamSetId == attempt.ExamSetId && o.IsCorrect)
           .Select(o => new { o.QuestionSetId, o.Id, o.QuestionSet.Points })
           .ToDictionaryAsync(o => o.QuestionSetId, o => o, cancellationToken);

        decimal totalScore = 0;
        decimal maxScore = await _context.QuestionSets.Where(q => q.ExamSetId == attempt.ExamSetId).SumAsync(q => q.Points, cancellationToken);

        // 3. Process each answer submitted by the user
        foreach (var submittedAnswer in request.Payload.Answers)
        {
            var questionId = submittedAnswer.Key;
            var selectedOptionId = submittedAnswer.Value;
            bool isCorrect = false;

            if (correctAnswers.TryGetValue(questionId, out var correctOption))
            {
                if (correctOption.Id == selectedOptionId)
                {
                    isCorrect = true;
                    totalScore += correctOption.Points;
                }
            }

            // Save the individual answer record
            _context.UserAnswers.Add(new UserAnswer
            {
                Id = Guid.NewGuid(),
                AttemptId = attempt.Id,
                QuestionId = questionId,
                SelectedOptionId = selectedOptionId,
                IsCorrect = isCorrect
            });
        }

        // 4. Finalize the Attempt
        attempt.EndTime = DateTime.UtcNow;
        attempt.TotalScore = totalScore;

        await _context.SaveChangesAsync(cancellationToken);

        bool passed = totalScore >= attempt.ExamSet.PassingScore;
        return new SubmitExamResponseDto(totalScore, maxScore, passed);
    }
}