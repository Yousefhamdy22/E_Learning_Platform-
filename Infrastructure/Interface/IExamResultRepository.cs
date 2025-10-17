using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Domain.Entities.Students;
using Infrastructure.Common.GenRepo;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IExamResultRepository : IGenaricRepository<ExamResult>
    {
        public Task<ExamResult> GetByStudentAndExamAsync(Guid studentId, Guid examId,
            CancellationToken ct = default);
        Task<List<ExamResult>> GetByStudentIdAsync(Guid studentId, CancellationToken ct);
        Task<ExamResult?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    }
}
