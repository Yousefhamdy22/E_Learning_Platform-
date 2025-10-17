using Domain.Common;
using Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Courses
{
    public class Module : AuditableEntity
    {
        public string Namw { get; set; }
        public string Description { get; set; }

        [ForeignKey("Course")]
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();


        private Module() { }
        private Module(string name, string description, Guid courseId)
        {
            Namw = name;
            Description = description;
            CourseId = courseId;
        }

        public static Result<Module> Create(string name, string description, Guid courseId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Module>.FromError(Error.Validation("Name is required"));
            var module = new Module(name, description, courseId);
            return Result<Module>.FromValue(module);
        }

        public void Update(string name, string description)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Namw = name;
            if (!string.IsNullOrWhiteSpace(description))
                Description = description;
        }


        public Result<Success> Delete()
        {
            if (Lectures.Any())
                return Result<Success>.FromError(Error.Validation("Cannot delete module with associated lectures."));
            return Result<Success>.FromValue(new Success());
        }
    }
}
