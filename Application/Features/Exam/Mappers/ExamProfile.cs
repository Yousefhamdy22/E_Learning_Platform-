using Application.Features.Exam.Commands.Exams.CreateExam;
using Application.Features.Exam.Dtos;
using Application.Features.Exam.Queries.Exams.GetExamsByCourseQuery;
using AutoMapper;
using Domain.Entities.Exams;
using Domain.Entities.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Exam.Mappers
{
    public class ExamProfile : Profile
    {

        public ExamProfile()
        {
            CreateMap<Domain.Entities.Exams.Exam, ExamDto>()
                 .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.ExamQuestions.OrderBy(eq => eq.QuestionOrder)))
                      .ReverseMap();    

            //CreateMap<Domain.Entities.Exams.Exam, ExamDto>().ReverseMap();
             CreateMap<CreateExamCommand , ExamDto>().ReverseMap();
             CreateMap<GetExamsByCourseQuery, ExamDto>().ReverseMap();
            CreateMap<Domain.Entities.Exams.Exam, GetExamsByCourseQuery>().ReverseMap();



            CreateMap<ExamResult, ExamResultDto>()
            //.ForMember(dest => dest.Tit, opt => opt.MapFrom(src => src.Exam.Title))
            //.ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.Name))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                src.SubmittedAt.HasValue ? src.SubmittedAt - src.StartedAt : (TimeSpan?)null));



            // For detailed exam result
            CreateMap<ExamResult, ExamResultDetailDto>()
                .ForMember(dest => dest.ExamTitle, opt => opt.MapFrom(src => src.Exam.Title))
                //.ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.na))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src =>
                    src.SubmittedAt.HasValue ? src.SubmittedAt - src.StartedAt : (TimeSpan?)null))
                .ForMember(dest => dest.StudentAnswers, opt => opt.MapFrom(src => src.StudentAnswers));




            CreateMap<StudentAnswer, StudentAnswerDetailDto>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.Text))
                .ForMember(dest => dest.SelectedAnswerText, opt => opt.MapFrom(src => src.SelectedAnswer.Text))
                .ForMember(dest => dest.QuestionPoints, opt => opt.MapFrom(src => src.Question.Points))
                .ForMember(dest => dest.PointsAwarded, opt => opt.MapFrom(src => src.IsCorrect ? src.Question.Points : 0))
                .ForMember(dest => dest.CorrectAnswers, opt => opt.MapFrom(src =>
                    src.Question.AnswerOptions.Where(a => a.IsCorrect).ToList()));


        }
    }
}
