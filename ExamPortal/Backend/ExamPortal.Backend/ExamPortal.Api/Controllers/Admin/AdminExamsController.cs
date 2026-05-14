using ExamPortal.Application.DTOs.Admin;
using ExamPortal.Application.Features.Exams.Commands;
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

    #region - Get All Exams for Admin
    [HttpGet("GetMyExams")]
    public async Task<IActionResult> GetMyExams()
    {
        // Extract the logged-in admin's ID directly from the JWT token
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var exams = await _mediator.Send(new GetAdminExamSetsQuery(Guid.Parse(userIdClaim)));
        return Ok(exams);
    }
    #endregion

    #region - Create Exam
    [HttpPost("CreateExam")]
    public async Task<IActionResult> CreateExam(CreateExamRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var examId = await _mediator.Send(new CreateExamCommand(Guid.Parse(userIdClaim), request));
        return Ok(new { ExamId = examId, Message = "Exam created successfully." });
    }
    #endregion

    #region - Delete Exam By Id
    [HttpDelete("DeleteExamById/{examId}")]
    public async Task<IActionResult> DeleteExam(Guid examId)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            await _mediator.Send(new DeleteExamCommand(examId, Guid.Parse(userIdClaim)));
            return Ok(new { Message = "Exam deleted successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message); // Returns a 403 Forbidden if they don't own the exam
        }
    }
    #endregion

    #region - Get Exam By Id
    [HttpGet("GetExamById/{examId:guid}")]
    public async Task<IActionResult> GetExamById(Guid examId)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            var exam = await _mediator.Send(new GetAdminExamByIdQuery(examId, Guid.Parse(userIdClaim)));
            return Ok(exam);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    #endregion

    #region - Update Exam By Id
    [HttpPut("UpdateExamById/{examId:guid}")]
    public async Task<IActionResult> UpdateExam(Guid examId, AdminExamDetailsDto payload)
    {
        try
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            await _mediator.Send(new UpdateExamContentCommand(examId, Guid.Parse(userIdClaim), payload));
            return Ok(new { Message = "Exam updated successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    #endregion
}