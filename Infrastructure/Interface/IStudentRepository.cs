using Domain.Common.Interface;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IStudentRepository : IGenaricRepository<Student>
    {
        Task<Student?> GetWithEnrollmentsAsync(Guid studentId , CancellationToken ct = default);
        Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid courseId);

        Task<bool> ExistsAsync(Guid studentId, CancellationToken ct = default);
    }
}
