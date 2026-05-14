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

    #region - Get Available Exams
    [HttpGet("AvailableExams")]
    public async Task<IActionResult> GetAvailableExams()
    {
        var exams = await _mediator.Send(new GetAvailableExamsQuery());

        if (!exams.Any())
        {
            return NoContent();
        }

        return Ok(exams);
    }
    #endregion

    #region - Get Exam Instructions
    [HttpGet("{id:guid}/Instructions")]
    public async Task<IActionResult> GetInstructions(Guid id)
    {
        try
        {
            var instructions = await _mediator.Send(new GetExamInstructionsQuery(id));
            return Ok(instructions);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    #endregion
}