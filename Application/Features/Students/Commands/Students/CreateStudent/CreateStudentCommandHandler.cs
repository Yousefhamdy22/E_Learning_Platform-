using Application.Common.Behaviours.Interfaces;
using Application.Features.Students.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities.Students;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Students.Commands.Students.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<StudentDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HybridCache _cache;
        private readonly ILogger<CreateStudentCommandHandler> _logger;
        private readonly ICurrentUserService _currentUserService;

        public CreateStudentCommandHandler(
            IUserService userService,
            IStudentRepository studentRepository,
            IMapper mapper,
            ILogger<CreateStudentCommandHandler> logger,
            IUnitOfWork unitOfWork,
            HybridCache cache,
            ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
            _userService = userService;
           
            _logger = logger;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<StudentDto>> Handle(CreateStudentCommand request, CancellationToken ct)
        {
            // Simple user extraction - just 2 lines!
            if (_currentUserService.UserId is not Guid userId)
                return Result<StudentDto>.FromError(Error.Unauthorized("User not authenticated"));


            var userResult = await _userService.GetUserByIdAsync(userId);
            if (!userResult.IsSuccess)
                return Result<StudentDto>.FromError(Error.NotFound("User.NotFound", "User not found"));

            var user = userResult.Value;

            _logger.LogInformation("Creating student for user {UserId}", userId);

            // Check if student already exists
            var existingStudent = await _studentRepository.GetByUserIdAsnc(userId, ct);
            if (existingStudent != null)
                return Result<StudentDto>.FromError(Error.Conflict("Student profile already exists"));

            // Create student
            var studentResult = Student.Create(
                userId,
                request.gender
              );

            if (!studentResult.IsSuccess)
                return Result<StudentDto>.FromError(Error.Failure("Error For Create Student "));


            var student = studentResult.Value;
            await _studentRepository.AddAsync(student, ct);
            await _unitOfWork.CommitAsync(ct);

            // Cache
            var studentDto = _mapper.Map<StudentDto>(student);
            await _cache.SetAsync($"student_{student.Id}", studentDto);
            await _cache.RemoveByTagAsync("students", ct);

            _logger.LogInformation("Student created with ID: {StudentId}", student.Id);
            //return Result<StudentDto>.FromValue(studentDto);

            return Result<StudentDto>.FromValue(new StudentDto
            {
                Id = student.Id,
                UserId = student.UserId,
               
                // User data from Auth Service
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            });
        }
    }
}
