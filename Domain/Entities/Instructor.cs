using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Courses;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Instructor : AuditableEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; private set; }
        public string Title { get; set; }
        public ApplicationUser User { get; private set; } = default!;
        public ICollection<Course> Courses { get; private set; } = new List<Course>();

       
        private Instructor() { }

        private Instructor(Guid userid , string title)
        {
            UserId = userid;
            Title = title;
        }

  
        public static Result<Instructor> Create(Guid userId , string title )
        {
            if (userId == Guid.Empty)
                return Result<Instructor>.FromError(
                    Error.Validation("Instructor.UserId.Empty", "UserId is required."));

           

            var instructor = new Instructor(userId , title);
            return Result<Instructor>.FromValue(instructor);
        }

       
        public Result<Success> UpdateProfile(string firstName, string lastName, string email)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result<Success>.FromError(Error.Validation("Instructor.FirstName.Empty", "First name is required."));

            if (string.IsNullOrWhiteSpace(lastName))
                return Result<Success>.FromError(Error.Validation("Instructor.LastName.Empty", "Last name is required."));

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return Result<Success>.FromError(Error.Validation("Instructor.Email.Invalid", "Valid email is required."));

       

            return Result<Success>.FromValue(new Success());
        }

        public void AddCourse(Course course)
        {
            if (course is null)
                throw new ArgumentNullException(nameof(course));

            Courses.Add(course);
        }

    }
}
