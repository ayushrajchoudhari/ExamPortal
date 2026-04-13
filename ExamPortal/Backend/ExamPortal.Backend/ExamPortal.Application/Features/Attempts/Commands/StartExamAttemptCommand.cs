using MediatR;
using ExamPortal.Domain.Entities;
using ExamPortal.Application.Interfaces;
using ExamPortal.Application.DTOs.User;

namespace ExamPortal.Application.Features.Attempts.Commands;

// We require both the Exam ID and the User ID to create an attempt
public record StartExamAttemptCommand(Guid ExamSetId, Guid UserId) : IRequest<StartAttemptResponseDto>;

public class StartExamAttemptCommandHandler : IRequestHandler<StartExamAttemptCommand, StartAttemptResponseDto>
{
    private readonly IExamPortalDbContext _context;

    public StartExamAttemptCommandHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<StartAttemptResponseDto> Handle(StartExamAttemptCommand request, CancellationToken cancellationToken)
    {
        var attempt = new ExamAttempt
        {
            Id = Guid.NewGuid(),
            ExamSetId = request.ExamSetId,
            UserId = request.UserId,
            StartTime = DateTime.UtcNow // Locks in the server time to prevent client-side manipulation
        };

        _context.ExamAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        return new StartAttemptResponseDto(attempt.Id);
    }
}