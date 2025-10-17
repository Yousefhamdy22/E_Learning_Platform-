using Application.Features.Instructors.Dto;
using Domain.Common.Results;
using MediatR;
using System;


namespace Application.Features.Instructors.Command.CreateInstructor
{
    public record CreateInstructorCommand(Guid userid , string title)
     : IRequest<Result<InstructorDto>>;

}
