using Domain.Common.Interface;
using Domain.Entities.Exams;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class QuestionRepository : GenericRepository<Question> , IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext context) : base(context)
        {

        }

        public Task<bool> ExistsAsync(string text, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Question?> GetByTextAsync(string text, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetQuestionsByExamIdAsync(Guid examId, CancellationToken ct = default)
        {

            return _context.Questions
                .Where(q => q.Id == examId)  // ExamId should be matched with ExamId property of Question
                .ToListAsync(ct);

        }
    }
}
