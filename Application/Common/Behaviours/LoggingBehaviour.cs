using Domain.Common.Results.Abstractions;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{

    public class LoggingBehavior<TRequest, TResponse>(
          ILogger<LoggingBehavior<TRequest, TResponse>> logger)
          : IPipelineBehavior<TRequest, TResponse>
          where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Starting request: {RequestName} with payload: {@Request}",
                requestName, request);

            try
            {
                var response = await next();

                if (response is IResult result && result.IsSuccess)
                {
                    _logger.LogInformation("Completed request: {RequestName} successfully", requestName);
                }
                else if (response is IResult resultFailure && !resultFailure.IsSuccess)
                {
                    _logger.LogWarning("Request {RequestName} failed with errors: {@Errors}",
                        requestName, resultFailure.Errors);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {RequestName} failed with exception", requestName);
                throw;
            }
        }
    }
}
