using Application.Common.Behaviours;
using Application.Common.Behaviours.Interfaces;
using Application.Features.Zoom.Mappers;
using Application.Services;
using Infrastructure.Helper.FileStorage.Validations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;


namespace Application
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {

           
            services.AddMediatR(cfg =>
            {
                
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
           
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpContextAccessor();
            //services.AddAutoMapper(typeof(ZoomMeetingProfile));

            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IInstructorService, InstructorService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<FileValidation>();
           


            return services;
        }
    }
}
