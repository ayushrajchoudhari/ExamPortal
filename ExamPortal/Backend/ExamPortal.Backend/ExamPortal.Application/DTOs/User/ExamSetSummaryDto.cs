namespace ExamPortal.Application.DTOs.User;

public record ExamSetSummaryDto(
    Guid Id,
    string Title,
    int DurationMinutes,
    decimal PassingScore
);
