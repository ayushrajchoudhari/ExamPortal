using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Interfaces;
using ExamPortal.Domain.Entities;
using MediatR;

namespace ExamPortal.Application.Features.Attempts.Commands;

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
            UserId = request.UserId,
            ExamSetId = request.ExamSetId,
            StartTime = DateTime.UtcNow // Use server UTC time to prevent client manipulation
        };

        _context.ExamAttempts.Add(attempt);
        await _context.SaveChangesAsync(cancellationToken);

        return new StartAttemptResponseDto(attempt.Id);
    }
}