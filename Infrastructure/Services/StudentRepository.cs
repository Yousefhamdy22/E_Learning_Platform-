using Domain.Entities.Courses;
using Domain.Entities.Students;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{


    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {

        public StudentRepository(ApplicationDbContext context):base(context)
        {

        }

        public async Task<Student?> GetWithEnrollmentsAsync(Guid studentId , CancellationToken ct = default)
        {
            return await _dbSet
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == studentId , ct);
        }

        public async Task<IEnumerable<Student>> GetStudentsByCourseAsync(Guid courseId)
        {
            return await _dbSet
                .Include(s => s.Enrollments)
                .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid studentId, CancellationToken ct = default)
        {
            return await _dbSet.AnyAsync(s => s.Id == studentId , ct);
        }
    }
    
}
