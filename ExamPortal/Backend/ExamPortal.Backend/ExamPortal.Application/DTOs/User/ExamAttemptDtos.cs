namespace ExamPortal.Application.DTOs.User;

// Objective 1 & 2: Instructions and Starting the Exam
public record InstructionDto(Guid Id, string Content, bool AgreementRequired);
public record StartAttemptRequestDto(Guid ExamSetId);
public record StartAttemptResponseDto(Guid AttemptId);

// Objective 4: Submitting Answers and Getting Results
public record SubmitExamRequestDto(Dictionary<Guid, Guid> Answers); // Maps QuestionId to SelectedOptionId
public record SubmitExamResponseDto(decimal TotalScore, decimal MaxScore, bool Passed);