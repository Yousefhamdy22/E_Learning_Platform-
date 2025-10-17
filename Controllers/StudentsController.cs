
using Application.Features.Students.Commands.StudentAnswer.SubmitStudentAnswerCommand;
using Application.Features.Students.Commands.Students.CreateStudent;
using Application.Features.Students.Dtos;
using Application.Features.Students.Queries.Students.GetStudentById;
using Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery;
using Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/Students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {

        private readonly IMediator _mediator;
        public StudentsController(IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost]
        public Task<IActionResult> CreateStudent([FromBody] CreateStudentCommand command, CancellationToken ct)
        {
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return Task.FromResult<IActionResult>(Unauthorized("Invalid user token"));


            return _mediator.Send(command, ct)
                .ContinueWith<IActionResult>(t =>
                {
                    var result = t.Result;
                    if (result.IsSuccess)
                        return CreatedAtAction(nameof(GetStudentById),
                            new { id = result.Value.Id }, result.Value);
                    return BadRequest(result.Errors);
                });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new GetStudentByIdQuery(id));

                if (result == null)
                    return NotFound($"Student with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("CourseEnrollment/{StudentId:guid}")]
        public async Task<IActionResult> EnrollmentCourse(Guid StudentId)
        {
            var result = await _mediator.Send(new GetStudentWithEnrollmentsQuery(StudentId));

            if (result == null)
                return NotFound($"No enrollments found for course with Id {StudentId}.");

            return Ok(result);
        }


        [HttpPost("submit")]
        public async Task<ActionResult<Result<Guid>>> SubmitAnswer([FromBody] SubmitStudentAnswerCommand request,
            CancellationToken ct)
        {
            
           
            var result = await _mediator.Send(request, ct);

            if (result.IsError)
                return BadRequest(result.Errors);

            return Ok(new { StudentAnswerId = result.Value });
        }



    }
} 