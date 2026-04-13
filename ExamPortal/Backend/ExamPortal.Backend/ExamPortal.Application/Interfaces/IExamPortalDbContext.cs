using Microsoft.EntityFrameworkCore;
using ExamPortal.Domain.Entities;

namespace ExamPortal.Application.Interfaces;

public interface IExamPortalDbContext
{
    DbSet<ExamSet> ExamSets { get; }
    DbSet<InstructionSet> InstructionSets { get; }
    DbSet<QuestionSet> QuestionSets { get; }
    DbSet<AnswerOption> AnswerOptions { get; }
    DbSet<User> Users { get; }
    DbSet<ExamAttempt> ExamAttempts { get; }
    DbSet<UserAnswer> UserAnswers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}