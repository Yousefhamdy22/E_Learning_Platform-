using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Exams;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities.Courses
{
    public class Course : AuditableEntity
    {

        #region Prop & Nav

        public const string TypeFree = "Free";

        public const string TypePaid = "Paid";
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string TypeStatus { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        //Nav 
        [ForeignKey("Instructor")]
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = default!;

        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>(); 
        public ICollection<Module> Modules { get; set; } = new List<Module>();

        #endregion


        #region Logic

        private Course() { } 

        private Course(string title, string? description, string Type , DateTimeOffset? 
            startDate, DateTimeOffset? endDate, decimal price, Guid instructorId)
        {
            Title = title;
            Description = description;
            TypeStatus = Type;
            StartDate = startDate;
            EndDate = endDate;
            Price = price;
            InstructorId = instructorId;
        }

        public static Result<Course> Create(string title, string? description, string typeStatus,
       DateTimeOffset? startDate, DateTimeOffset? endDate, decimal price, Guid instructorId)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Course>.FromError(Error.Failure("Title is required"));

            if (string.IsNullOrWhiteSpace(typeStatus))
                return Result<Course>.FromError(Error.Failure("Type is required"));

            if (price < 0)
                return Result<Course>.FromError(Error.Failure("Price cannot be negative"));

            if (instructorId == Guid.Empty)
                return Result<Course>.FromError(Error.Failure("Instructor ID is required"));

            var course = new Course(title, description, typeStatus, startDate, endDate, price, instructorId);
            return Result<Course>.FromValue(course);
        }


        public Result<Success> Update(string title, string? description,
             DateTimeOffset? startDate, DateTimeOffset? endDate, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Success>.FromError(new Error());

            Title = title;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            Price = price;

            return Result<Success>.FromValue(new Success());
        }

        public Result<Success> Delete()
        {
            if (Lectures.Count > 0)
                return Result<Success>.FromError(
                    Error.Validation("Course.Delete.Invalid", "Cannot delete course with associated lectures."));
            if (Enrollments.Count > 0)
                return Result<Success>.FromError(
                    Error.Validation("Course.Delete.Invalid", "Cannot delete course with enrolled students."));
            if (Exams.Count > 0)
                return Result<Success>.FromError(
                    Error.Validation("Course.Delete.Invalid", "Cannot delete course with associated exams."));
            return Result<Success>.FromValue(new Success());
        }

        public Result<Success> AssignInstructor(Guid instructorId)
        {
            if (instructorId == Guid.Empty)
                return Result<Success>.FromError(
                    Error.Validation("Course.InstructorId.Empty", "InstructorId is required."));
            InstructorId = instructorId;
            return Result<Success>.FromValue(new Success());
        }

        #endregion
    }
}
