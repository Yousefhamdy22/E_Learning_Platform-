using Application.Features.Enrollments.Commands.CreateEnrollment;
using Application.Features.Enrollments.Commands.RemoveEnrollment;
using Application.Features.Enrollments.Commands.UpdateEnrollment;
using Application.Features.Enrollments.Dto;
using Application.Features.Enrollments.Quires.GetEnrollmentByIdQuery;
using Application.Features.Exam.Commands.Exams.CreateExam;
using Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery;
using Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/Enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public EnrollmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentDetailsDto>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEnrollmentByIdQuery(id));
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
        }

        [HttpPost]
        public async Task<ActionResult<EnrollmentDto>> Create([FromBody] EnrollInCourseCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok();
           
        }


        //[HttpGet("Details/{id}")]
        //public async Task<ActionResult<EnrollmentDetailsDto>> GetDetailsById(Guid id)
        //{
        //    var result = await _mediator.Send(new GetStudentWithEnrollmentsQuery(id));
        //    return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
        //}

        [HttpPut("{id}")]
        public async Task<ActionResult<EnrollmentDto>> Update(Guid id, [FromBody] UpdateEnrollmentStatusCommand command)
        {
            if (id != command.EnrollmentId)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

      
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(CancelEnrollmentCommand command)
        {
            var result = await _mediator.Send(new CancelEnrollmentCommand(command.EnrollmentId 
                ,command.CancellationReason));
            return result.IsSuccess ? NoContent() : NotFound(result.Errors);
        }



    }
}
