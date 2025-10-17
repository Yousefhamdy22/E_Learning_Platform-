using Domain.Common.Interface;
using Domain.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ICourseRepository : IGenaricRepository<Course>
    {
        Task<Course?> GetWithLecturesAsync(Guid courseId);
        Task<IEnumerable<Course>> GetActiveCoursesAsync();

    }
}
