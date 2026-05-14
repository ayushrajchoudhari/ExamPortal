using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Attempts.Queries;

public record ValidateExamTokenQuery(Guid ExamSetId, string Token) : IRequest<bool>;

public class ValidateExamTokenQueryHandler : IRequestHandler<ValidateExamTokenQuery, bool>
{
    private readonly IExamPortalDbContext _context;

    public ValidateExamTokenQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ValidateExamTokenQuery request, CancellationToken cancellationToken)
    {
        return await _context.ExamSets
           .AnyAsync(e => e.Id == request.ExamSetId && e.SecretToken == request.Token, cancellationToken);
    }
}