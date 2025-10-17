using Application.Features.Course.Dtos;
using Domain.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours.Interfaces
{
    public interface ICourseService
    {
        //public Task<Result<CourseDto>> CreateCourseAsync(CreateCourseDto dto, CancellationToken ct);
        public Task<Result<CourseDto>> UpdateCourseAsync(Guid id, CourseDto dto, CancellationToken ct);
        public Task<Result<Success>> DeleteCourseAsync(Guid id, CancellationToken ct);
        public Task<Result<CourseDto>> GetCourseByIdAsync(Guid id, CancellationToken ct);
        //public Task<Result<Success>> AssignInstructorAsync(Guid courseId, Guid instructorId, CancellationToken ct);
    }
}
