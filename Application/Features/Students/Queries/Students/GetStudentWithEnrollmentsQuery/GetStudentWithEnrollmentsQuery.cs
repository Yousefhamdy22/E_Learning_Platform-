using Application.Common.Behaviours.Interfaces;
using Application.Features.Students.Dtos;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Queries.Students.GetStudentWithEnrollmentsQuery
{
    public record GetStudentWithEnrollmentsQuery(Guid StudentId)
     : IRequest<Result<StudentWithEnrollmentsDto>>, ICachedQuery
    {
        public string CacheKey => $"student-enrollments:{StudentId}";
        public TimeSpan Expiration => TimeSpan.FromMinutes(15);
        public string[]? Tags => new[] { "students", $"student:{StudentId}", "enrollments" };
    }
}
