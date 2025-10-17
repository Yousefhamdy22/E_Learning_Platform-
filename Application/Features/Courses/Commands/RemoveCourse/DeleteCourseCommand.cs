using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Courses.Commands.RemoveCourse
{
    public record DeleteCourseCommand(Guid Id) : IRequest<Result<Success>>;
}
