using Application.Common.Behaviours.Interfaces;
using Application.Features.Course.Dtos;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.RemoveCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/Courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMediator _mediator;
        public CoursesController(ICourseService courseService , IMediator mediator)
        {
            _courseService = courseService;
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<Result<IActionResult>> CreateCourse([FromBody] CreateCourseCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
                return Ok(result.Value);

            return Error.Failure("Error with creating Course ");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseById(Guid id, CancellationToken ct)
        {
            var result = await _courseService.GetCourseByIdAsync(id, ct);
            if (result.IsSuccess)
                return Ok(result.Value);
            return NotFound(result.Errors);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseCommand command, CancellationToken ct)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command, ct);
            if (result.IsSuccess)
                return NoContent();
            return BadRequest(result.Errors);

        }

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteCourseCommand(id);
            var result = _mediator.Send(command);

            if (result.IsCompletedSuccessfully)
                return Task.FromResult<IActionResult>(NoContent());

            return Task.FromResult<IActionResult>(NotFound(result.Result.Errors));

        }
    }
}
