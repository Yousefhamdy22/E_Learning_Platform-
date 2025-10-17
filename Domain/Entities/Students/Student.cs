using Domain.Common;
using Domain.Common.Results;
using Domain.Entities.Exams;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities.Students
{
    public class Student : AuditableEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; private set; }
      
        public string? Gender { get; set; }
      

        // Nav properties
        public ApplicationUser User { get; private set; } = default!;
        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();
        public ICollection<ExamResult> ExamResults { get; private set; } = new List<ExamResult>();

      
        private Student() { }

       
        private Student(Guid userId, string gender )
        {
            UserId = userId;
            Gender = gender;

        }

     
        public static Result<Student> Create(Guid userId, string gender)
        {
          
            if (userId == Guid.Empty)
                return Result<Student>.FromError(
                    Error.Validation("StudentProfile.UserId.Empty", "UserId is required."));

            var student = new Student(userId, gender);

           
            return Result<Student>.FromValue(student);
        }


        public Result<Student> Update( string gender)
        {
          

            if (string.IsNullOrWhiteSpace(gender))
                return Result<Student>.FromError(
                    Error.Validation("StudentProfile.gender.Empty", "gender is required."));

            Gender = gender;

            return Result<Student>.FromValue(this);
        }
    }
}
