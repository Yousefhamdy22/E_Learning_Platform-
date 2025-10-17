using Application.Common.Behaviours.Interfaces;
using Application.Features.Instructors.Command.CreateInstructor;
using Application.Features.Instructors.Command.DeleteInstructor;
using Application.Features.Instructors.Command.UpdateInstructor;
using Application.Features.Instructors.Dto;
using Application.Features.Instructors.Queries.GetInstructorByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/Instructors")]
    [ApiController]
    public class InstructorsController : ControllerBase
    {

        private readonly IMediator _mediator;

        public InstructorsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        public async Task<IActionResult> CreateInstructor(CreateInstructorCommand instr)
        {


           
            var command = new CreateInstructorCommand(
              instr.userid,
               instr.title
                );
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstructor(Guid id)
        {
          
            var command = new UpdateInstructorCommand { InstructorId = id };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(Guid id)
        {
            var command = new DeleteInstructorCommand(id);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInstructorById(Guid id)
        {
            var query = new GetInstructorByIdQuery(id);
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result.Value) : NotFound();
        }
    }
}
