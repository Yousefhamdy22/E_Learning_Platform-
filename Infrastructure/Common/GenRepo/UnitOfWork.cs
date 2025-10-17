using Domain.Common.Interface;
using Domain.Entities;
using Domain.Entities.Courses;
using Domain.Entities.Exams;
using Domain.Entities.Identity;
using Domain.Entities.Students;
using Domain.Entities.ZoomMeeting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Common.GenRepo
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private IDbContextTransaction? _transaction;

       

        public IGenaricRepository<ApplicationUser> ApplicationUsers { get; private set; }

        public IGenaricRepository<Student> Students { get; private set; }

        public IGenaricRepository<StudentAnswer> StudentAnswers { get; private set; }

        public IGenaricRepository<Course> Courses { get; private set; }

        public IGenaricRepository<Exam> Exams { get; private set; }

        public IGenaricRepository<Question> Questions { get; private set; }

        public IGenaricRepository<AnswerOption> AnswerOptions { get; private set; }

        public IGenaricRepository<Lecture> Lectures { get; private set; }

        public IGenaricRepository<Instructor> Instructors { get; private set; }

        public IGenaricRepository<Enrollment> Enrollments { get; private set; }

        public IGenaricRepository<ExamResult> ExamResults { get; private set; }

        public IGenaricRepository<ZoomRecording> ZoomRecordes 
            { get; private set; }

        public IGenaricRepository<ZoomMeeting> ZoomMeetings { get; private set; }

        public UnitOfWork(
            ApplicationDbContext context,
            ILogger<UnitOfWork> logger
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Students = new GenericRepository<Student>(context);
            Exams = new GenericRepository<Exam>(context);
            StudentAnswers = new GenericRepository<StudentAnswer>(context);
            Courses = new GenericRepository<Course>(context);
            Lectures = new GenericRepository<Lecture>(context);
            ExamResults = new GenericRepository<ExamResult>(context);
            Questions = new GenericRepository<Question>(context);
            Instructors = new GenericRepository<Instructor>(context);
            Enrollments = new GenericRepository<Enrollment>(context);
            ApplicationUsers = new GenericRepository<ApplicationUser>(context);
            ZoomMeetings = new GenericRepository<ZoomMeeting>(context);
            ZoomRecordes = new GenericRepository<ZoomRecording>(context);




        }
        public async Task<int> CommitAsync(CancellationToken ct)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while committing changes");
                throw;
            }
        }


        public async Task RollbackAsync()
        {
            try
            {
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during rollback");
                throw;
            }
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }


        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
        }
        public void Dispose()
        {
              //Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
