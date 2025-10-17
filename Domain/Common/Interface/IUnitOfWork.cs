using Domain.Entities;
using Domain.Entities.Courses;
using Domain.Entities.Exams;
using Domain.Entities.Identity;
using Domain.Entities.Students;
using Domain.Entities.ZoomMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Common.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenaricRepository<Student> Students { get; }
        IGenaricRepository<StudentAnswer> StudentAnswers { get; }
        IGenaricRepository<Course> Courses { get; }
        IGenaricRepository<Exam> Exams { get; }
        IGenaricRepository<Question> Questions { get; }
        IGenaricRepository<AnswerOption> AnswerOptions { get; }
        IGenaricRepository<Lecture> Lectures { get; }
        IGenaricRepository<Instructor> Instructors { get; }
        IGenaricRepository<Enrollment> Enrollments { get; }
        IGenaricRepository<ExamResult> ExamResults { get; }
        IGenaricRepository<ZoomMeeting> ZoomMeetings { get; }
        IGenaricRepository<ZoomRecording> ZoomRecordes { get; }

        
         

       

        Task<int> CommitAsync(CancellationToken ct);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task RollbackAsync();

    }
}
