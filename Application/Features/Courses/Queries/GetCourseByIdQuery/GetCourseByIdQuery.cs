using Application.Features.Course.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Courses.Queries.GetCourseByIdQuery
{
    public record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDto>>;
}
