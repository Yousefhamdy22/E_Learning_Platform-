using Application.Features.Course.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Courses.Commands.CreateCourse
{
    public record CreateCourseCommand(
      string Title, string? Description, string TypeStatus, DateTimeOffset? StartDate,
      DateTimeOffset? EndDate, decimal Price, Guid InstructorId)
      : IRequest<Result<CourseDto>>;

}
