namespace ExamPortal.Application.DTOs.User;

public record InstructionDto(
    Guid Id, 
    string Content, 
    bool AgreementRequired
);

public record StartAttemptRequestDto(
    Guid ExamSetId
);

public record StartAttemptResponseDto(
    Guid AttemptId
);

public record SubmitExamRequestDto(
    Dictionary<Guid, Guid> Answers
);

public record SubmitExamResponseDto(
    decimal TotalScore, 
    decimal MaxScore, 
    bool Passed
);

public record ExamResultDto(
    Guid AttemptId,
    string ExamTitle,
    decimal TotalScore,
    decimal PassingScore,
    bool Passed,
    DateTime StartTime,
    DateTime? EndTime
);

public record ValidateTokenRequestDto(
    Guid ExamSetId, string Token
);