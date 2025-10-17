using Application.Features.Instructors.Dto;
using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Instructors.Queries.GetInstructorByIdQuery
{
    public record GetInstructorByIdQuery(Guid InstructorId) : IRequest<Result<InstructorDto>>;

}
