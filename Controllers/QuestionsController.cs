using Application.Features.Exam.Commands.Questions.CreateQuestions;
using Application.Features.Exam.Commands.Questions.RemoveQuestions;
using Application.Features.Exam.Commands.Questions.UpdateQuestions;
using Application.Features.Exam.Queries.Questions.GetAllQuestionsQuery;
using Application.Features.Exam.Queries.Questions.GetQuestionByIdQuery;
using Application.Features.Exam.Queries.Questions.GetQuestionsByExamIdQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/Questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {

        private readonly IMediator _mediator;
        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromForm] CreateQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsError)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(Guid id)
        {
            var query = new GetQuestionByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var query = new GetAllQuestionsQuery();
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);


        }

        [HttpGet("ByExam/{examId}")]
        public async Task<IActionResult> GetQuestionsByExamId(Guid examId)
        {
            var query = new GetQuestionsByExamIdQuery(examId);
            var result = await _mediator.Send(query);
            if (result.IsError)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
           var command = new RemoveQuestionCommand(id);
              var result = await _mediator.Send(command);
            if (result.IsError)
                {
                return NotFound(result.Errors);
            }
            return NoContent();

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(Guid id, [FromBody] UpdateQuestionCommand command)
        {
            if (id != command.QuestionId)
            {
                return BadRequest("ID mismatch");
            }
            var result = await _mediator.Send(command);
            if (result.IsError)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Value);
        }
    }
}
