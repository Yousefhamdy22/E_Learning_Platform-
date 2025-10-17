using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Instructors.Command.DeleteInstructor
{
    public record DeleteInstructorCommand(Guid Id) : IRequest<Result<bool>>;

}
