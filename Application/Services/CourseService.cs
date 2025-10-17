using Application.Common.Behaviours.Interfaces;
using Application.Features.Course.Dtos;
using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.RemoveCourse;
using Application.Features.Courses.Commands.UpdateCourse;
using Application.Features.Courses.Queries.GetCourseByIdQuery;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IMediator _mediator;

        public CourseService(IMediator mediator)
        {
            _mediator = mediator;
        }

        //public Task<Result<CourseDto>> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct)
        //{
        //    var command = new CreateCourseCommand(
        //        dto.Title, dto.Description, dto.StartDate, dto.EndDate, dto.Price, dto.InstructorId);
        //    return _mediator.Send(command, ct);
        //}

       
        public Task<Result<CourseDto>> UpdateCourseAsync(Guid id, UpdateCourseDto dto, CancellationToken ct)
        {
            var command = new UpdateCourseCommand(
                id, dto.Title, dto.Description, dto.StartDate, dto.EndDate, dto.Price);
            return _mediator.Send(command, ct);
        }

        public Task<Result<Success>> DeleteCourseAsync(Guid id, CancellationToken ct)
        {
            var command = new DeleteCourseCommand(id);
            return _mediator.Send(command, ct);
        }

        public Task<Result<CourseDto>> GetCourseByIdAsync(Guid id, CancellationToken ct)
        {
            var query = new GetCourseByIdQuery(id);
            return _mediator.Send(query, ct);
        }

        public Task<Result<CourseDto>> UpdateCourseAsync(Guid id, CourseDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        //public Task<Result<Success>> AssignInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct)
        //{
        //    var command = new AssignInstructorCommand(courseId, instructorId);
        //    return _mediator.Send(command, ct);
        //}
    }

}
