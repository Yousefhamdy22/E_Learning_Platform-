﻿using Domain.Common.Results.Abstractions;
using System.Threading.Tasks;
using Domain.Common.Results;
using FluentValidation;
using System.Threading;
using MediatR;

namespace Application.Common.Behaviours
{

    public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
        : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
            where TResponse : IResult
    {
        private readonly IValidator<TRequest>? _validator = validator;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            if (_validator is null)
            {
                return await next(ct);
            }

            var validationResult = await _validator.ValidateAsync(request, ct);

            if (validationResult.IsValid)
            {
                return await next();
            }

            var errors = validationResult.Errors
                .ConvertAll(error => Error.Validation(
                    code: error.PropertyName,
                    description: error.ErrorMessage));

            return (dynamic)errors;
        }
    }
}
