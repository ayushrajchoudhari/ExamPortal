namespace ExamPortal.Application.DTOs.Admin;

public record AdminExamSetSummaryDto(
    Guid Id,
    string Title,
    int DurationMinutes,
    decimal PassingScore,
    bool IsPublic,
    int QuestionCount,
    string? SecretToken
);