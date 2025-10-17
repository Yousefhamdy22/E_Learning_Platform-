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
    public interface IAnswerOptionRepository : IGenaricRepository<AnswerOption> 
    {


        Task<List<AnswerOption>> GetByQuestionIdAsync(Guid questionId, CancellationToken ct);
    }
}
