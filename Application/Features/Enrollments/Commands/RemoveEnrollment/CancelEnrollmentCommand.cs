using Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Commands.RemoveEnrollment
{
    public record CancelEnrollmentCommand : IRequest<Result<bool>>
    {
        public Guid EnrollmentId { get; init; }
        public string? CancellationReason { get; init; }

        public CancelEnrollmentCommand( 
            Guid enrollmentId = default,
            string? cancellationReason = null
            )
        {       
            EnrollmentId = Guid.Empty;
            CancellationReason = string.Empty;
        }
    }
}
