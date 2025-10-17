using Application.Common.Behaviours.Interfaces;
using Application.Features.Students.Dtos;
using Domain.Common.Results;
using MediatR;
using System;


namespace Application.Features.Students.Queries.Students.GetStudentById
{
    public record GetStudentByIdQuery(Guid StudentId)
     : IRequest<Result<StudentDto>>, ICachedQuery
    {
        public string CacheKey => $"student:{StudentId}";
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
        public string[]? Tags => new[] { "students", $"student:{StudentId}" };
    }
}
