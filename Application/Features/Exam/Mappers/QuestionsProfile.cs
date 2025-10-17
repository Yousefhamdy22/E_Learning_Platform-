using Application.Features.Exam.Dtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Mappers
{
    public class QuestionsProfile : Profile
    {

        public QuestionsProfile() { 
         
           CreateMap<Domain.Entities.Exams.Question, QuestionDto>()
                .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.Points)).ReverseMap();






        }
    }
}
