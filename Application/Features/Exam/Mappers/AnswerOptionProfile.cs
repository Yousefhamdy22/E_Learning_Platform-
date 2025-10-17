using Application.Features.Exam.Dtos;
using AutoMapper;
using Domain.Entities.Exams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Mappers
{
    public class AnswerOptionProfile : Profile
    {
        public AnswerOptionProfile()
        {
            CreateMap<AnswerOption, AnswerOptionDto>().ReverseMap();
        }
    }
}
