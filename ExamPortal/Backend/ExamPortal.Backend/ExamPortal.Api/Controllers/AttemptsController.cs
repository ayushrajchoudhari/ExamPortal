using ExamPortal.Application.DTOs.User;
using ExamPortal.Application.Features.Attempts.Commands;
using ExamPortal.Application.Features.Attempts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExamPortal.Api.Controllers;

[ApiController]

[Authorize]
public class AttemptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttemptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region - Start Exam Attempt
    [HttpPost("start")]
    public async Task<IActionResult> StartAttempt(StartAttemptRequestDto request)
    {
        // Securely extract the logged-in user's ID directly from the JWT claims
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized(new { message = "Invalid token claims." });
        }

        var command = new StartExamAttemptCommand(request.ExamSetId, Guid.Parse(userIdClaim));
        var result = await _mediator.Send(command);

        return Ok(result);
    }
    #endregion

    #region - Get Active Exam Questions
    [HttpGet("{attemptId:guid}/questions")]
    public async Task<IActionResult> GetQuestions(Guid attemptId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var questions = await _mediator.Send(new GetActiveExamQuestionsQuery(attemptId, Guid.Parse(userId!)));
            return Ok(questions);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    #endregion

    #region - Submit Exam Attempt
    [HttpPost("{attemptId:guid}/submit")]
    public async Task<IActionResult> SubmitExam(Guid attemptId, SubmitExamRequestDto request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mediator.Send(new SubmitExamCommand(attemptId, Guid.Parse(userId!), request));
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    #endregion
}