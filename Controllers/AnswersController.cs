using Application.Features.Exam.Commands.Answers.CreateAnswer;
using Application.Features.Exam.Queries.Answers.GetAnswerOptionsByQuestionIdQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/Answers")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AnswersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswerOption([FromBody] CreateAnswerOptionCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetAnswerOptionsByQuestionId),
                    new { questionId = command.QuestionId }, result.Value);
            return BadRequest(result.Errors);
        }

        [HttpGet("ByQuestion/{questionId}")]
        public async Task<IActionResult> GetAnswerOptionsByQuestionId(Guid questionId, CancellationToken ct)
        {
            var query = new GetAnswerOptionsByQuestionIdQuery(questionId);
         
            var result = await _mediator.Send(query, ct);
            if (result.IsSuccess)
                return Ok(result.Value);
            return NotFound(result.Errors);
        }






    }
}
