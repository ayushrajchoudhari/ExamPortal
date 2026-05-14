namespace ExamPortal.Application.DTOs.Admin;

public record CreateExamRequestDto(
    string Title,
    int DurationMinutes,
    decimal PassingScore,
    string InstructionContent,
    bool IsPublic
);