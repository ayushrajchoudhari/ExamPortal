using ExamPortal.Application.Features.Exams.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamPortal.Api.Controllers;

[ApiController]
[Route("api/exams")]
[Authorize]
public class ExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableExams()
    {
        var exams = await _mediator.Send(new GetAvailableExamsQuery());

        if (!exams.Any())
        {
            return NoContent();
        }

        return Ok(exams);
    }
}