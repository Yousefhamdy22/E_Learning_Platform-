using Application.Features.Instructors.Dto;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Instructors.Command.UpdateInstructor
{
    public class UpdateInstructorCommand : IRequest<Result<InstructorDto>>
    {
        public Guid InstructorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}
