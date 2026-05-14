namespace ExamPortal.Application.DTOs.Admin;

public record AdminExamDetailsDto(
    Guid Id,
    string Title,
    int DurationMinutes,
    decimal PassingScore,
    bool IsPublic,
    string? SecretToken,
    string InstructionContent,
    List<AdminQuestionDto> Questions
);

public record AdminQuestionDto(
    Guid? Id, // Nullable: Will be null if it's a new question added from UI
    string QuestionText,
    decimal Points,
    int DisplayOrder,
    List<AdminAnswerOptionDto> Options
);

public record AdminAnswerOptionDto(
    Guid? Id, // Nullable: Will be null if it's a new option added from UI
    string OptionText,
    bool IsCorrect
);