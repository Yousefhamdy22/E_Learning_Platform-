using Domain.Common.Interface;
using Domain.Entities.Courses;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
       
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Course?> GetWithLecturesAsync(Guid courseId)
        {
            return await _dbSet.Include(c => c.Lectures)
                               .Include(c => c.Enrollments)
                               .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
        {
            return await _dbSet.Where(c => c.StartDate <= DateTime.UtcNow &&
                                           (c.EndDate == null || c.EndDate >= DateTime.UtcNow))
                               .ToListAsync();
        }

    }
}
