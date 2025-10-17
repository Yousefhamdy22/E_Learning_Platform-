using Application.Common.Behaviours.Interfaces;
using Domain.Common.Results.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using System;

namespace Application.Common.Behaviours
{

    public class CachingBehavior<TRequest, TResponse>(
        HybridCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly HybridCache _cache = cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
           
            if (request is not ICachedQuery cachedRequest)
            {
                _logger.LogDebug("Request {RequestName} is not cacheable", typeof(TRequest).Name);
                return await next(ct);
            }

            _logger.LogInformation("Checking cache for {RequestName} with key: {CacheKey}",
                typeof(TRequest).Name, cachedRequest.CacheKey);

            
            try
            {
                var result = await _cache.GetOrCreateAsync(
                    cachedRequest.CacheKey,
                    async (ct) =>
                    {
                        _logger.LogInformation("Cache miss for {RequestName}. Executing handler...",
                            typeof(TRequest).Name);
                        return await next(ct);
                    },
                    options: CreateCacheOptions(cachedRequest),
                    tags: cachedRequest.Tags,
                    ct);

                
                if (result is IResult { IsSuccess: false })
                {
                    _logger.LogWarning("Request {RequestName} failed. Removing from cache if exists.",
                        typeof(TRequest).Name);
                    await _cache.RemoveAsync(cachedRequest.CacheKey, ct);
                }
                else
                {
                    _logger.LogInformation("Cache hit or successful execution for {RequestName}",
                        typeof(TRequest).Name);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in caching behavior for {RequestName}", typeof(TRequest).Name);
                
                return await next(ct);
            }
        }

        private HybridCacheEntryOptions CreateCacheOptions(ICachedQuery cachedRequest)
        {
            return new HybridCacheEntryOptions
            {
                Expiration = cachedRequest.Expiration.TotalSeconds > 0
                    ? cachedRequest.Expiration
                    : TimeSpan.FromMinutes(10),
            };
        }
    }

}
