//using Application;
//using Application.Common.AuthDto;
//using Domain.Entities.Identity;
//using Infrastructure;
//using Infrastructure.Data;
//using Infrastructure.Helper;
//using Infrastructure.ZoomServices;
//using Infrastructure.ZoomServices.Dtos;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using System.Reflection;
//using System.Text.Json.Serialization;

//namespace E_Learning_Platform
//{
//    public class Program
//    {
//        public static async Task Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            #region HttpClient 
            
//            builder.Services.AddHttpClient();
//            #endregion

//            #region Sql Connection

//            builder.Services.AddDbContext<ApplicationDbContext>(options =>
//            {
//                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//            });
//            #endregion

//            #region IoC 
         
//            builder.Services.AddInfrastructureDependencies().AddServiceDependencies()
//             .AddServiceRegisteration(builder.Configuration);
//            builder.Services.Configure<ZoomSettings>(builder.Configuration.GetSection("ZoomSettings"));
//            builder.Services.Configure<ZoomTokenResponse>(builder.Configuration.GetSection("ZoomToken"));

        
//            #region Cors

//            var Cors = "_cors";
//            builder.Services.AddCors(options =>
//              options.AddPolicy(name: Cors, builder =>

//              {
//                  builder.AllowAnyHeader();
//                  builder.AllowAnyMethod();
//                  builder.AllowAnyOrigin();
//              }
//                  )
//            );

//            #endregion

//            #endregion

//            #region MiddelWare


//            builder.Services.AddControllers()
//               .AddJsonOptions(options =>
//               {
//                   options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//                   options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//               });
            
//            //builder.Services.AddEndpointsApiExplorer();
//            //builder.Services.AddSwaggerGen();  
//            #region Swagger Config

//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen(c =>
//            {
//                c.SwaggerDoc("v1", new OpenApiInfo
//                {
//                    Title = "E-Learning API",
//                    Version = "v1",
//                    Description = "API for E-Learning Platform"
//                });

//                // Add JWT Bearer authentication support
//                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//                {
//                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header,
//                    Type = SecuritySchemeType.ApiKey,
//                    Scheme = "Bearer"
//                });

//                c.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        new string[] {}
//                    }
//                });

//                // Include XML comments if available
//                try
//                {
//                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
//                    if (File.Exists(xmlPath))
//                    {
//                        c.IncludeXmlComments(xmlPath);
//                    }
//                }
//                catch
//                {
//                    // Ignore if XML comments aren't available
//                }
//            });
//            #endregion



//            var app = builder.Build();

//            #region Role

//            using (var scope = app.Services.CreateScope())
//            {
//                var services = scope.ServiceProvider;
//                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
//                var logger = services.GetRequiredService<ILogger<Program>>();

//                // Seed roles using the enum
//                var roles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>();
//                foreach (var role in roles)
//                {
//                    var roleName = role.ToString();
//                    if (!await roleManager.RoleExistsAsync(roleName))
//                    {
//                         await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
//                        logger.LogInformation("Role {Role} created", roleName);
//                    }
//                }

//                logger.LogInformation("Role seeding completed");
//            }



//            #endregion

//            app.UseSwagger();
//            app.UseSwaggerUI(c =>
//            {
//                c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Learning API V1");
//                c.RoutePrefix = "swagger"; // This makes it available at /swagger

//                // For production, you might want to hide the Swagger UI
//                // but keep the JSON available for API consumers
//                if (!app.Environment.IsDevelopment())
//                {
//                    c.DocumentTitle = "API Documentation - Production";
//                }
//            });
//            //if (app.Environment.IsDevelopment())
//            //{
//            //    app.UseSwagger();
//            //    app.UseSwaggerUI();
//            //}

//            app.UseHttpsRedirection();
          

//            app.UseAuthorization();


//            app.MapControllers();

//            app.Run();

//            #endregion
//        }
//    }
//}
