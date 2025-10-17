using Application.Features.Course.Dtos;
using AutoMapper;


namespace Application.Features.Courses.Mappers
{
    public class CourseProfile : Profile
    {

        public CourseProfile()
        {
            CreateMap<CourseDto, Domain.Entities.Courses.Course>()
                   .ForMember(dest => dest.TypeStatus, opt => opt.MapFrom(src => src.TypeStatus))
                   .ReverseMap();


            CreateMap<CreateCourseDto, Domain.Entities.Courses.Course>();

            CreateMap<UpdateCourseDto, Domain.Entities.Courses.Course>();


        }
    }
}
