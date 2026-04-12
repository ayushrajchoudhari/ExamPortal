namespace ExamPortal.Application.DTOs.User;

public record LoginRequestDto(string Email, string Password);
public record AuthResponseDto(string Token, Guid UserId, string Role);