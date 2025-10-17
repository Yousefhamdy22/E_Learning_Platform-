using Application.Features.Students.Dtos;
using Domain.Common.Results;
using MediatR;
using System;

namespace Application.Features.Students.Commands.Students.CreateStudent
{
    public record CreateStudentCommand(
      
       string gender
    ) : IRequest<Result<StudentDto>>;
    
}
