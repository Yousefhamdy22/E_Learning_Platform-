using Domain.Common.Interface;
using Domain.Entities.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface ILectureRespository : IGenaricRepository<Lecture>
    {
    }
}
