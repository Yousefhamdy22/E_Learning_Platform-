using Application.Features.Exam.Commands.Exams.StartExam;
using Application.Features.Exam.Commands.Exams.SubmitExam;
using Application.Features.Exam.Queries.ExamResult.GetExamResultDetailQuery;
using Application.Features.Exam.Queries.ExamResult.GetStudentExamResultsQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/ExamResults")]
    [ApiController]
    public class ExamResultsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ExamResultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartExam([FromBody] StartExamCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsError)
                return BadRequest(result.TopError);

            return Ok(new { ExamResultId = result.Value });
        }


        [HttpPost("submit-exam")]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (result.IsError)
                return BadRequest();

            return Ok(result.Value);
        }

        [HttpGet("student/{studentId:guid}")]
        public async Task<IActionResult> GetStudentExamResults(Guid studentId, CancellationToken ct)
        {
            var query = new GetStudentExamResultsQuery { StudentId = studentId };
            var result = await _mediator.Send(query, ct);

            return Ok(result.Value);
        }

        [HttpGet("{examResultId:guid}")]
        public async Task<IActionResult> GetExamResultDetail(Guid examResultId, CancellationToken ct)
        {
            var query = new GetExamResultDetailQuery { ExamResultId = examResultId };
            var result = await _mediator.Send(query, ct);

            if (result.IsError)
                return NotFound(result.TopError);

            return Ok(result.Value);
        }

      
    }
}
