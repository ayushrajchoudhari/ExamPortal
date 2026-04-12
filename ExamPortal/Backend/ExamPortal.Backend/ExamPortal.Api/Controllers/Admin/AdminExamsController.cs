using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExamPortal.Application.DTOs.Admin;

namespace ExamPortal.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/exams")]
public class AdminExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateExam(CreateExamRequestDto request)
    {
        return Ok(new { Message = "Exam creation endpoint ready to be wired to MediatR." });
    }
}