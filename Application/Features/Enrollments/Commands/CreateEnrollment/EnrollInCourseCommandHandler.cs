using Application.Features.Enrollments.Dto;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities;
using Domain.Entities.Courses;
using Infrastructure.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Commands.CreateEnrollment
{
    public class EnrollInCourseCommandHandler : IRequestHandler<EnrollInCourseCommand, Result<EnrollmentDto>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentRepository _studentRepository;        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollInCourseCommandHandler(IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository, IStudentRepository studentRepository,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<EnrollmentDto>> Handle(EnrollInCourseCommand request, CancellationToken ct)
        {
            // Validate student exists
            var student = await _studentRepository.GetByIdAsync(request.StudentId, ct);
            if (student == null)
                return Result<EnrollmentDto>.FromError(Error.NotFound("Student.NotFound", "Student not found"));

            // Validate course exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId, ct);
            if (course == null)
                return Result<EnrollmentDto>.FromError(Error.NotFound("Course.NotFound", "Course not found"));

            // Check for existing enrollment - this will now work correctly
            var existingEnrollment = await _enrollmentRepository
                .GetByStudentAndCourseAsync(request.StudentId, request.CourseId, request.EnrollmentDate, ct);

            if (existingEnrollment != null)
                return Result<EnrollmentDto>.FromError(
                    Error.Conflict("Enrollment.Exists", "Already enrolled in this course"));
            

            // Determine status: Free courses are Active immediately, Paid courses need admin approval
            string status = course.TypeStatus == Domain.Entities.Courses.Course.TypeFree
              ? Enrollment.StatusActive
              : Enrollment.StatusPending;

            // Create enrollment
            var enrollmentResult = Enrollment.Create(request.StudentId, request.CourseId, status);
            if (!enrollmentResult.IsSuccess)
                return Result<EnrollmentDto>.FromError(Error.Failure("Enrollment.Create.Failed",
                    "Failed to create enrollment"));

            var enrollment = enrollmentResult.Value;
            await _enrollmentRepository.AddAsync(enrollment, ct);
            await _unitOfWork.CommitAsync(ct);

            return Result<EnrollmentDto>.FromValue(_mapper.Map<EnrollmentDto>(enrollment));
        }
       
    }
}
