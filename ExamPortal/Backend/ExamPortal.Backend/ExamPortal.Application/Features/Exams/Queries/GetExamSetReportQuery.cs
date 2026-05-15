using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExamPortal.Application.Features.Exams.Queries;

public record GetExamSetReportQuery(Guid ExamId, Guid AdminId) : IRequest<AdminExamReportDto>;

public class GetExamSetReportQueryHandler : IRequestHandler<GetExamSetReportQuery, AdminExamReportDto>
{
    private readonly IExamPortalDbContext _context;

    public GetExamSetReportQueryHandler(IExamPortalDbContext context)
    {
        _context = context;
    }

    public async Task<AdminExamReportDto> Handle(GetExamSetReportQuery request, CancellationToken cancellationToken)
    {
        var exam = await _context.ExamSets
           .Include(e => e.ExamAttempts)
               .ThenInclude(a => a.User)
           .AsNoTracking()
           .FirstOrDefaultAsync(e => e.Id == request.ExamId && e.CreatedByUserId == request.AdminId, cancellationToken);

        if (exam == null)
            throw new UnauthorizedAccessException("Exam not found or access denied.");

        var candidates = exam.ExamAttempts.Select(a => new CandidateResultDto(
            a.User.Email,
            a.StartTime,
            a.EndTime,
            a.TotalScore,
            a.TotalScore >= exam.PassingScore
        )).OrderByDescending(c => c.StartTime).ToList();

        return new AdminExamReportDto(exam.Title, exam.PassingScore, candidates);
    }
}