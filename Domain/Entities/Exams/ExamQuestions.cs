using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Exams
{
    public class ExamQuestions
    {

        public Guid ExamId { get; set; }
        public Guid QuestionId { get; set; }

        public  int QuestionOrder { get; set; }



        // Navigation properties
        public Exam Exam { get; set; } = default!;
        public Question Question { get; set; } = default!;


        public static ExamQuestions Create(Guid examId, Guid questionId, int order)
        {
            return new ExamQuestions { ExamId = examId, QuestionId = questionId, QuestionOrder = order };
        }
    }
}
