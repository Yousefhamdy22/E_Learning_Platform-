
using Domain.Entities.Courses;
using Infrastructure.Common.GenRepo;
using Infrastructure.Data;
using Infrastructure.Interface;

namespace Infrastructure.Services
{

    public class LectureRespository : GenericRepository<Lecture>, ILectureRespository
    {

        public LectureRespository(ApplicationDbContext context) : base(context)
        {
        }
    }
   
}
