using Application.Features.Enrollments.Dto;
using AutoMapper;
using Domain.Common.Results;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Enrollments.Quires.GetEnrollmentByIdQuery
{
    public class GetEnrollmentByIdQueryHandler : IRequestHandler<GetEnrollmentByIdQuery, Result<EnrollmentDetailsDto>>
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IMapper _mapper;
        private readonly HybridCache _cache;

        public GetEnrollmentByIdQueryHandler(
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper,
            HybridCache cache)
        {
            _enrollmentRepository = enrollmentRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task<Result<EnrollmentDetailsDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken ct)
        {
            var cacheKey = $"enrollment_{request.EnrollmentId}";

            var dto = await _cache.GetOrCreateAsync(
                cacheKey,
                async (ct) => {
                    var enrollmentResult = await _enrollmentRepository
                        .GetByIdWithDetailsAsync(request.EnrollmentId, ct);

                    if (!enrollmentResult.IsSuccess)
                        return null;

                  
                    return _mapper.Map<EnrollmentDetailsDto>(enrollmentResult.Value);
                },
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) },
                tags: new[] { "enrollments" },
                ct
            );

            return dto != null
                ? Result<EnrollmentDetailsDto>.FromValue(dto)
                : Result<EnrollmentDetailsDto>.FromError(Error.NotFound("Enrollment.NotFound"));
        }
        //public async Task<Result<EnrollmentDetailsDto>> Handle(GetEnrollmentByIdQuery request, CancellationToken ct)
        //{

        //    var cacheKey = $"enrollment_{request.EnrollmentId}";

        //    var enrollmentResult = await _cache.GetOrCreateAsync(
        //        cacheKey,
        //        async (ct) => {

        //            var enrollment = await _enrollmentRepository.
        //                 GetByIdWithDetailsAsync(request.EnrollmentId, ct);


        //            //var result = await _repository.GetByIdAsync(request.EnrollmentId, ct);

        //            // ✅ Extract the value from Result<Enrollment> or return null
        //            return enrollment.IsSuccess ? enrollment.Value : null;
        //        },
        //        new HybridCacheEntryOptions
        //        {
        //            Expiration = TimeSpan.FromMinutes(30)
        //        },
        //        tags: new[] { "enrollments" },
        //        ct
        //    );

        //    if (enrollmentResult == null)
        //        return Result<EnrollmentDetailsDto>.FromError(Error.NotFound("Enrollment.NotFound"));

        //    // ✅ Map Enrollment entity to EnrollmentDetailsDto
        //    var dto = _mapper.Map<EnrollmentDetailsDto>(enrollmentResult);
        //    return Result<EnrollmentDetailsDto>.FromValue(dto);
        //    //var cacheKey = $"Enrollment_{request.EnrollmentId}";

        //    //var dto = await _cache.GetOrCreateAsync(
        //    //    cacheKey,
        //    //    async (ct) =>
        //    //    {

        //    //        var enrollment = await _enrollmentRepository.
        //    //             GetByIdWithDetailsAsync(request.EnrollmentId, ct);

        //    //        return enrollment.IsSuccess ? enrollment.Value : null;

        //    //        //return _mapper.Map<EnrollmentDetailsDto>(enrollment);
        //    //    },
        //    //    new HybridCacheEntryOptions
        //    //    {
        //    //        Expiration = TimeSpan.FromMinutes(30)
        //    //    },
        //    //    tags: new[] { "Enrollments" },
        //    //    cancellationToken: ct 

        //    //);

        //    //if (dto == null)
        //    //    return Result<EnrollmentDetailsDto>.FromError(
        //    //        Error.NotFound("Enrollment.NotFound", "Enrollment not found"));

        //    //var dto = _mapper.Map<EnrollmentDetailsDto>(enrollmentResult);
        //    //return Result<EnrollmentDetailsDto>.FromValue(dto);
        //    ////return Result<EnrollmentDetailsDto>.FromValue(dto);
        //}
    }
}
