using Application.Common.Behaviours.Interfaces;
using Application.Features.Course.Dtos;
using Application.Features.Lecture.Dtos;
using Application.Features.Students.Commands.Students.CreateStudent;
using Application.Features.Students.Commands.Students.UpdateStudent;
using Application.Features.Students.Dtos;
using Application.Features.Students.Queries.Students.GetStudentById;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Students;
using Infrastructure.Data;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Application.Services
{
    public class StudentService : IStudentService
    {

        #region DI Injection

        
        private readonly IStudentRepository _studentRepository;
        private readonly IGenaricRepository<Student> _student;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        #endregion

        #region Ctors Injection

        
        public StudentService(IStudentRepository studentRepository, 
            IEnrollmentRepository enrollmentRepository , IGenaricRepository<Student> student
            , IMapper mapper, ILogger<StudentService> logger,   IMediator mediator) 
        { 
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _student = student;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }
        #endregion

    
        #region Student Command

       
        //public Task<Result<StudentDto>> CreateStudentAsync(CreateStudentDto dto, CancellationToken ct)
        //{


        //    var command = new CreateStudentCommand(
        //        EnrollmentDate
        //       );

        //    return _mediator.Send(command, ct);
        //}

        //public Task<Result<StudentDto>> UpdateStudentAsync(Guid id, StudentDto dto)
        //{
        //   var command = new UpdateStudentCommand(
        //        id,
        //        dto.FirstName,
        //        dto.LastName,
        //        dto.Email);
        //    return _mediator.Send(command);
        //}


        #endregion

        #region Student Queries 
        public Task<IEnumerable<StudentDto>> GetStudentsByCourseAsync(Guid courseId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public  async Task<StudentDto> GetStudentAsync(Guid studentId, CancellationToken ct)
        {
            var query = new GetStudentByIdQuery(studentId);
            var result = await _mediator.Send(query, ct);
            return result.Value;
        }

        public  Task<StudentWithEnrollmentsDto> GetWithEnrollmentsAsync(Guid studentId, CancellationToken ct)
        {
            var query = new GetStudentByIdQuery(studentId);
            var result =  _mediator.Send(query, ct);
              return Task.FromResult(new StudentWithEnrollmentsDto());

        }
        #endregion

        #region UnHandeld

       
        public Task<Result<Success>> EnrollInCourseAsync(Guid studentId, Guid courseId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CourseDto>> GetCoursesAsync(Guid studentId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<LectureDto>> GetLecturesAsync(Guid studentId, Guid courseId)
        {
            throw new NotImplementedException();
        }

        //public Task<IReadOnlyList<ZoomDto>> GetExamsAsync(Guid studentId, Guid courseId)
        //{
        //    throw new NotImplementedException();
        //}


        public async Task<StudentDto> GetStudentsAsyc(CancellationToken ct)
        {
            await _student.GetAllAsync(ct);
            return new StudentDto();

        }
        #endregion


        #region StudentWithCourse

        public Task GetStudentsByCourse (Guid courseId, CancellationToken ct = default)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            return _studentRepository.GetStudentsByCourseAsync(courseId);
        }
        public Task GetStudentWithEnrollments (Guid studentId, CancellationToken ct = default)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            return _studentRepository.GetWithEnrollmentsAsync(studentId , ct);
        }

       

        #endregion

        #region Student Checks
        public async Task<StudentDto> GetStudentById(Guid studentId, CancellationToken ct = default)
        {
            var query = new GetStudentByIdQuery(studentId);
            var result = await _mediator.Send(query);

            return result.Value;
        }
        public async Task<bool> StudentExistsAsync(Guid studentId, CancellationToken ct = default)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            return await _studentRepository.ExistsAsync(studentId, ct);
        }
        public async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId,
            CancellationToken ct = default)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            return await _enrollmentRepository.ExistsAsync(studentId, courseId, ct);
        }


        #endregion

    }
}
