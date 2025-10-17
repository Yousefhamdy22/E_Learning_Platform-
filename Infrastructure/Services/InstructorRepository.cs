using Domain.Common.Interface;
using Domain.Entities;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class InstructorRepository : GenericRepository<Instructor> , IInstructorRepository
    {
        private readonly ApplicationDbContext _context;
        public InstructorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
