using Application.Common.Behaviours.Interfaces;
using Application.Features.Instructors.Command.CreateInstructor;
using Application.Features.Instructors.Command.DeleteInstructor;
using Application.Features.Instructors.Command.UpdateInstructor;
using Application.Features.Instructors.Dto;
using Application.Features.Instructors.Queries.GetInstructorByIdQuery;
using AutoMapper;
using Domain.Common.Interface;
using Domain.Common.Results;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IInstructorRepository _instructorRepository;
        private readonly IGenaricRepository<Instructor> _instructor;
        private readonly ILogger<InstructorService> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public InstructorService(IInstructorRepository instructorRepository,
                                 IGenaricRepository<Instructor> instructor,
                                 IMapper mapper,
                                 ILogger<InstructorService> logger,
                                 IMediator mediator)
        {
            _instructorRepository = instructorRepository;
            _instructor = instructor;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        //public Task<Result<InstructorDto>> CreateInstructorAsync(CreateInstructorDto dto, CancellationToken ct)
        //{
        //    var command = new CreateInstructorCommand(
        //        Guid.Empty,
        //        dto.FirstName,
        //        dto.LastName,
        //        dto.Email);

        //    return _mediator.Send(command, ct);
        //}

        //public Task<Result<InstructorDto>> UpdateInstructorAsync(Guid id, InstructorDto dto)
        //{
        //    var command = new UpdateInstructorCommand(
        //        id,
        //        dto.FirstName,
        //        dto.LastName,
        //        dto.Email);

        //    return _mediator.Send(command);
        //}

        public Task<Result<bool>> DeleteInstructorAsync(Guid id)
        {
            var command = new DeleteInstructorCommand(id);
            return _mediator.Send(command);
        }

        public Task<Result<InstructorDto>> GetInstructorByIdAsync(Guid id)
        {
            var query = new GetInstructorByIdQuery(id);
            return _mediator.Send(query);
        }
    }

}
