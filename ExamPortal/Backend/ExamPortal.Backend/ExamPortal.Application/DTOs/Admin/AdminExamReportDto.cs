namespace ExamPortal.Application.DTOs.Admin;

public record CandidateResultDto(
    string Email, DateTime StartTime, 
    DateTime? EndTime, 
    decimal? TotalScore, 
    bool Passed
);
public record AdminExamReportDto(
    string ExamTitle, 
    decimal PassingScore, 
    List<CandidateResultDto> Candidates
);