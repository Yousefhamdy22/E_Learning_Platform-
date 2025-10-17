using Application.Features.Instructors.Dto;
using System.Threading.Tasks;
using Domain.Common.Results;
using System.Threading;
using System;

namespace Application.Common.Behaviours.Interfaces
{
    public interface IInstructorService
    {
        //public Task<Result<InstructorDto>> CreateInstructorAsync(CreateInstructorDto dto, CancellationToken ct);
        //public Task<Result<InstructorDto>> UpdateInstructorAsync(Guid id, InstructorDto dto);
        public Task<Result<bool>> DeleteInstructorAsync(Guid id);
        public Task<Result<InstructorDto>> GetInstructorByIdAsync(Guid id);


    }
}
