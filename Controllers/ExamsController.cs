using Application.Features.Exam.Commands.Exams.CreateExam;
using Application.Features.Exam.Queries.Exams.GetExamByIdQuery;
using Application.Features.Exam.Queries.Exams.GetExamsByCourseQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {

        private readonly IMediator _mediator;
        public ExamsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsError)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExamById(Guid id)
        {
            var query = new GetExamByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }


        [HttpGet("ByCourse/{courseId}")]
        public async Task<IActionResult> GetExamsByCourseId(Guid courseId)
        {
            var query = new GetExamsByCourseQuery(courseId);
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }

    }
}
