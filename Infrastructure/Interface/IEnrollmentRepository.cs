using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IEnrollmentRepository : IGenaricRepository<Enrollment> 
    {
        Task<Enrollment> GetByStudentAndCourseAsync(Guid studentId, Guid courseId 
         ,DateTimeOffset enrollmentDate , CancellationToken ct );

        Task<Result< Enrollment>> GetByIdWithDetailsAsync(Guid enrollmentId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
    }
}
