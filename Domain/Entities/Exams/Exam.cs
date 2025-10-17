using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Courses;
using Domain.Entities.Exams.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Domain.Entities.Exams
{
    public class Exam : AuditableEntity
    {
        #region Properties

        public string Title { get; private set; }
        public string? Description { get; private set; }

        public Guid CourseId { get; private set; }
        public Course Course { get; private set; } = default!; 

        public int DurationMinutes { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTimeOffset StartDate { get; private set; }
        public DateTimeOffset EndDate { get; private set; }

        //

        private readonly List<ExamQuestions> _examQuestions = new();
        public IReadOnlyList<ExamQuestions> ExamQuestions => _examQuestions.AsReadOnly();

        private readonly List<ExamResult> _examResults = new();
        public IReadOnlyCollection<ExamResult> ExamResults => _examResults.AsReadOnly();

        #endregion

        #region Constructors

       
        private Exam() { }

        private Exam(
            string title,
            string? description,
            Guid courseId,
            int durationMinutes,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {
            Title = title;
            Description = description;
            CourseId = courseId;
            DurationMinutes = durationMinutes;
            StartDate = startDate;
            EndDate = endDate;
        }

        #endregion

        #region Factory

        public static Result<Exam> Create(
            string title,
            string? description,
            Guid courseId,
            int durationMinutes,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {

            if (string.IsNullOrWhiteSpace(title))
                return Result<Exam>.FromError(Error.Validation("Exam.Title.Empty", "Title cannot be empty."));

            if (startDate >= endDate)
                return Result<Exam>.FromError(Error.Validation("Exam.Date.Invalid", "Start date must be before end date."));

            if (durationMinutes <= 0)
                return Result<Exam>.FromError(Error.Validation("Exam.Duration.Invalid", "Duration must be positive."));

            var exam = new Exam(title, description, courseId, durationMinutes, startDate, endDate);
            exam.AddDomainEvent(new ExamCreatedDomainEvent(exam.Id, courseId, startDate));
            return Result<Exam>.FromValue(exam);
        }

        #endregion

        #region Behaviors

        public Result<Success> Update(
            string title,
            string? description,
            int durationMinutes,
            DateTimeOffset startDate,
            DateTimeOffset endDate)
        {

            if (string.IsNullOrWhiteSpace(title))
                return Result<Success>.FromError(Error.Validation("Course.Title.Empty", "Title is required"));

            if (string.IsNullOrWhiteSpace(title))
                return Result<Success>.FromError(Error.Validation("Exam.Title.Empty", "Title cannot be empty."));

            if (startDate >= endDate)
                return Result<Success>.FromError(Error.Validation("Exam.Date.Invalid", "Start date must be before end date."));

            if (durationMinutes <= 0)
                return Result<Success>.FromError(Error.Validation("Exam.Duration.Invalid", "Duration must be positive."));

            Title = title;
            Description = description;
            DurationMinutes = durationMinutes;
            StartDate = startDate;
            EndDate = endDate;

            return Result<Success>.FromValue(new Success());
        }


        public Result<Success> RemoveExam()
        {
            if (_examResults.Count > 0)
                return Result<Success>.FromError(
                    Error.Validation("Exam.Remove.Invalid", "Cannot remove an exam with student results."));

            // Rule 2: Cannot remove an exam that already started
            if (StartDate <= DateTimeOffset.UtcNow)
                return Result<Success>.FromError(
                    Error.Validation("Exam.Remove.Invalid", "Cannot remove an exam that has already started."));

            return Result<Success>.FromValue(new Success());
        }
        public Result<Success> CompleteExam(Guid studentId)
        {
            // Maybe verify student is allowed and exam is active, etc.
            // Save results or mark as complete here.

            //AddDomainEvent(new ExamCompletedDomainEvent(this.Id, studentId,  DateTimeOffset.UtcNow));

            return Result<Success>.FromValue(new Success());
        }
        public Result<Success> AutoGrade(Guid studentId)
        {
            // Run grading logic here, calculate score...

           // AddDomainEvent(new ExamAutoGradedDomainEvent(this.Id, studentId, score));

            return Result<Success>.FromValue(new Success());
        }


        public void AddExamQuestion(ExamQuestions examQuestion)
        {
            _examQuestions.Add(examQuestion);
        }

      

        //public Result<Success> RemoveQuestion(Guid questionId)
        //{
        //    var question = _questions.FirstOrDefault(q => q.Id == questionId);
        //    if (question == null)
        //        return Result<Success>.FromError(Error.Validation("Exam.Question.NotFound", "Question not found."));

        //    _questions.Remove(question);
        //    return Result<Success>.FromValue(new Success());
        //}


        // Optionally 
        public Result<Success> AddExamResult(ExamResult result)
        {
            if (result == null)
                return Result<Success>.FromError(Error.Validation("Exam.Result.Null", "Result cannot be null."));

            _examResults.Add(result);
            return Result<Success>.FromValue(new Success());
        }

        #endregion
    }
}
