using Application.Features.Instructors.Dto;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Instructors.Mappers
{
    public class InstructorProfile : Profile
    {
        public InstructorProfile()
        {
            CreateMap<InstructorDto, Instructor>().ReverseMap();

            CreateMap<CreateInstructorDto, Instructor>();

      
        }
    }
}
