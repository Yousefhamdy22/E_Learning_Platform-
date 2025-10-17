using Application.Features.Course.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public record UpdateCourseCommand(Guid Id, string Title, string? Description,
    DateTimeOffset? StartDate, DateTimeOffset? EndDate, decimal Price)
    : IRequest<Result<CourseDto>>;
}
