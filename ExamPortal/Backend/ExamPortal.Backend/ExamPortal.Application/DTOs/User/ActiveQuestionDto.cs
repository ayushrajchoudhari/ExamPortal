namespace ExamPortal.Application.DTOs.User;

public record ActiveQuestionDto(
    Guid Id,
    string QuestionText,
    decimal Points,
    List<ActiveAnswerOptionDto> Options
);

public record ActiveAnswerOptionDto(
    Guid Id,
    string OptionText
);
