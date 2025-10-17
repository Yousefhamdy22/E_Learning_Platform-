using Domain.Common.Interface;
using Domain.Entities.Exams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IQuestionRepository : IGenaricRepository<Question>
    {
        public Task<List<Question>> GetQuestionsByExamIdAsync(Guid examId, 
            CancellationToken cancellationToken = default);

          Task<Question?> GetByTextAsync(string text, CancellationToken ct);

        Task<bool> ExistsAsync(string text, CancellationToken ct);

    }
}
