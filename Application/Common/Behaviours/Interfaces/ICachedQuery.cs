﻿using MediatR;
using System;


namespace Application.Common.Behaviours.Interfaces
{
    public interface ICachedQuery
    {
        string CacheKey { get; }
        string[] Tags { get; }
        TimeSpan Expiration { get; }
    }

    public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;
}
