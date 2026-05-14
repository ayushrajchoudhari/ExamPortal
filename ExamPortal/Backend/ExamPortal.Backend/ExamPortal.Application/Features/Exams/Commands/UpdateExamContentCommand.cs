using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Interfaces;
using ExamPortal.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Commands;

public record UpdateExamContentCommand(Guid ExamId, Guid AdminId, AdminExamDetailsDto Payload) : IRequest<bool>;

public class UpdateExamContentCommandHandler : IRequestHandler<UpdateExamContentCommand, bool>
{
    private readonly IExamPortalDbContext _context;

    public UpdateExamContentCommandHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateExamContentCommand request, CancellationToken cancellationToken)
    {
        var exam = await _context.ExamSets
           .Include(e => e.InstructionSet)
           .Include(e => e.QuestionSets)
               .ThenInclude(q => q.AnswerOptions)
           .FirstOrDefaultAsync(e => e.Id == request.ExamId && e.CreatedByUserId == request.AdminId, cancellationToken);

        if (exam == null)
            throw new UnauthorizedAccessException("Exam not found or you do not have permission.");

        // 1. Update Core Properties
        exam.Title = request.Payload.Title;
        exam.DurationMinutes = request.Payload.DurationMinutes;
        exam.PassingScore = request.Payload.PassingScore;
        exam.IsPublic = request.Payload.IsPublic;

        if (exam.InstructionSet != null)
        {
            exam.InstructionSet.Content = request.Payload.InstructionContent;
        }

        // 2. Perform a robust merge of Questions
        var incomingQuestionIds = request.Payload.Questions.Where(q => q.Id.HasValue).Select(q => q.Id!.Value).ToList();

        // Remove deleted questions
        var questionsToRemove = exam.QuestionSets.Where(q => !incomingQuestionIds.Contains(q.Id)).ToList();
        _context.QuestionSets.RemoveRange(questionsToRemove);

        foreach (var incQ in request.Payload.Questions)
        {
            if (incQ.Id.HasValue && exam.QuestionSets.Any(q => q.Id == incQ.Id.Value))
            {
                // Update existing question
                var existingQ = exam.QuestionSets.First(q => q.Id == incQ.Id.Value);
                existingQ.QuestionText = incQ.QuestionText;
                existingQ.Points = incQ.Points;
                existingQ.DisplayOrder = incQ.DisplayOrder;

                // Merge Options
                var incOptionIds = incQ.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();
                var optionsToRemove = existingQ.AnswerOptions.Where(o => !incOptionIds.Contains(o.Id)).ToList();
                foreach (var oToRemove in optionsToRemove) existingQ.AnswerOptions.Remove(oToRemove);

                foreach (var incO in incQ.Options)
                {
                    if (incO.Id.HasValue && existingQ.AnswerOptions.Any(o => o.Id == incO.Id.Value))
                    {
                        var existingO = existingQ.AnswerOptions.First(o => o.Id == incO.Id.Value);
                        existingO.OptionText = incO.OptionText;
                        existingO.IsCorrect = incO.IsCorrect;
                    }
                    else
                    {
                        // Add new option to existing question
                        existingQ.AnswerOptions.Add(new AnswerOption { Id = Guid.NewGuid(), QuestionSetId = existingQ.Id, OptionText = incO.OptionText, IsCorrect = incO.IsCorrect });
                    }
                }
            }
            else
            {
                // Add brand new question with options
                var newQ = new QuestionSet
                {
                    Id = Guid.NewGuid(),
                    ExamSetId = exam.Id,
                    QuestionText = incQ.QuestionText,
                    Points = incQ.Points,
                    DisplayOrder = incQ.DisplayOrder,
                    AnswerOptions = incQ.Options.Select(o => new AnswerOption
                    {
                        Id = Guid.NewGuid(),
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };
                exam.QuestionSets.Add(newQ);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}