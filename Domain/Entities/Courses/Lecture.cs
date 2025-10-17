using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Courses.Event;
using System;

namespace Domain.Entities.Courses
{
    public class Lecture : AuditableEntity
    {
        #region Properties & Navigation

        public string Title { get; private set; } = default!;
        public string? ContentUrl { get; private set; }  

        public Guid CourseId { get; private set; }
        public Course Course { get; private set; } = default!;

        public DateTimeOffset ScheduledAt { get; private set; }
        public TimeSpan Duration { get; private set; }
        public bool IsCompleted { get; private set; } = false;

        #endregion



       
        #region Constructors

    
        private Lecture() { }

        private Lecture(string title, Guid courseId, DateTimeOffset scheduledAt, TimeSpan duration)
        {
            Title = title;
            CourseId = courseId;
            ScheduledAt = scheduledAt;
            Duration = duration;
        }

        #endregion

        #region Factory Methods

        public static Result<Lecture> Create(string title, Guid courseId,
            DateTimeOffset scheduledAt, TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Lecture>.FromError(
                    Error.Validation("Lecture.Title.Empty", "Lecture title is required."));

            if (courseId == Guid.Empty)
                return Result<Lecture>.FromError(
                    Error.Validation("Lecture.CourseId.Empty", "CourseId is required."));

            if (duration.TotalMinutes <= 0)
                return Result<Lecture>.FromError(
                    Error.Validation("Lecture.Duration.Invalid", "Duration must be greater than 0."));

            var lecture = new Lecture(title, courseId, scheduledAt, duration);
            lecture.AddDomainEvent(new LectureCreatedDomainEvent(lecture.Id, courseId, scheduledAt));
            return Result<Lecture>.FromValue(lecture);
        }

        #endregion

        #region Domain Behaviors

        public Result<Success> MarkAsCompleted(string contentUrl)
        {
            if (string.IsNullOrWhiteSpace(contentUrl))
                return Result<Success>.FromError(
                    Error.Validation("Lecture.ContentUrl.Empty", "ContentUrl is required."));

            ContentUrl = contentUrl;
            IsCompleted = true;

             AddDomainEvent(new LectureEvents(Id, ContentUrl));

            return Result<Success>.FromValue(new Success());

           
        }

        public Result<Success> Reschedule(DateTimeOffset newSchedule)
        {
            if (newSchedule <= DateTimeOffset.UtcNow)
                return Result<Success>.FromError(
                    Error.Validation("Lecture.Schedule.Invalid", "Schedule must be in the future."));

            ScheduledAt = newSchedule;

            AddDomainEvent(new LectureRescheduledDomainEvent(Id, newSchedule));
            return Result<Success>.FromValue(new Success());
        }

      

        #endregion
    }
}
