using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Interfaces;
using ExamPortal.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ExamPortal.Application.Features.Exams.Commands;

public record CreateExamCommand(Guid AdminId, CreateExamRequestDto Payload) : IRequest<Guid>;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, Guid>
{
    private readonly IExamPortalDbContext _context;

    public CreateExamCommandHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        string? secretToken = null;

        // If the exam is private, generate a secure 8-character token
        if (!request.Payload.IsPublic)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            secretToken = RandomNumberGenerator.GetString(chars, 8);
        }

        // Retrieve the TenantId of the Admin creating the exam
        var adminUser = await _context.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.Id == request.AdminId, cancellationToken);

        var examId = Guid.NewGuid();

        var exam = new ExamSet
        {
            Id = examId,
            TenantId = adminUser?.TenantId ?? Guid.Empty,
            Title = request.Payload.Title,
            DurationMinutes = request.Payload.DurationMinutes,
            PassingScore = request.Payload.PassingScore,
            IsActive = true,
            CreatedByUserId = request.AdminId,
            IsPublic = request.Payload.IsPublic,
            SecretToken = secretToken
        };

        var instructions = new InstructionSet
        {
            Id = Guid.NewGuid(),
            ExamSetId = examId,
            Content = request.Payload.InstructionContent,
            AgreementRequired = true
        };

        _context.ExamSets.Add(exam);
        _context.InstructionSets.Add(instructions);

        await _context.SaveChangesAsync(cancellationToken);

        return examId;
    }
}