using Domain.Common.Interface;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Infrastructure.Common.GenRepo;
using Infrastructure.GoogleDeriveServices;
using Infrastructure.Helper.FileStorage;
using Infrastructure.Interface;
using Infrastructure.Services;
using Infrastructure.ZoomServices;
using Infrastructure.ZoomServices.RecordingService;
using Infrastructure.ZoomServices.RecordingService.BackgroundTask;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ILectureRespository, LectureRespository>();
            services.AddScoped<ICourseRepository, CourseRepository>();   
            
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            services.AddScoped<IZoomMeetingRepository, ZoomMeetingRepository>();
            services.AddScoped<IInstructorRepository, InstructorRepository>();

            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IStudentAnswerRepository, StudentAnswerRepository>();

            services.AddScoped<IExamResultRepository, ExamResultRepository>();
            services.AddScoped<IAnswerOptionRepository, AnswerOptionRepository>();

            services.AddScoped<IZoomService, ZoomService>();
            services.AddScoped<IZoomAuthService, ZoomAuthService>();
           
            services.AddScoped<DriveService>(provider =>
            {
                // Method 1: Using Service Account (Recommended for server-side apps)
                var credential = GoogleCredential.FromFile("path/to/your/service-account-key.json")
                    .CreateScoped(DriveService.Scope.Drive);

                // Method 2: Using OAuth 2.0 (if you need user context)
                // var credential = ... [your OAuth2 credential logic]

                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Your Application Name",
                });
            });

            services.AddScoped<IRecordingService, RecordingService>();
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
            services.AddScoped<IFileService, FileService>();

            services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10), // Distributed( L2, L3)
                LocalCacheExpiration = TimeSpan.FromSeconds(30), // Local Memory L1
            });

           
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenaricRepository<>), typeof(GenericRepository<>));

            return services;


        }


    }
}
