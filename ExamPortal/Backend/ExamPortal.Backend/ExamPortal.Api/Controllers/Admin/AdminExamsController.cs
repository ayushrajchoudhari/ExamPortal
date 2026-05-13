using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Features.Exams.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamPortal.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/exams")]
[Authorize]
public class AdminExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetMyExams()
    {
        // Extract the logged-in admin's ID directly from the JWT token
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var exams = await _mediator.Send(new GetAdminExamSetsQuery(Guid.Parse(userIdClaim)));
        return Ok(exams);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateExam(CreateExamRequestDto request)
    {
        return Ok(new { Message = "Exam creation endpoint ready to be wired to MediatR." });
    }
}