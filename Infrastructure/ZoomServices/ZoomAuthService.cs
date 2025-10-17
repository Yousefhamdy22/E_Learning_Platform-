﻿using Infrastructure.Helper;
using Infrastructure.ZoomServices.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.ZoomServices
{
    public class ZoomAuthService : IZoomAuthService
    {
        private readonly ZoomSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ZoomAuthService> _logger;

        private const string TokenCacheKey = "ZoomAccessToken";

        public ZoomAuthService(
            IOptions<ZoomSettings> options,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<ZoomAuthService> logger)
        {
            _settings = options.Value;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }
        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            if (_cache.TryGetValue(TokenCacheKey, out string cachedToken))
                return cachedToken;

            var client = _httpClientFactory.CreateClient();

            var requestUrl = $"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={_settings.AccountId}";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}")
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            request.Content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var tokenResult = JsonSerializer.Deserialize<ZoomTokenResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _cache.Set(TokenCacheKey, tokenResult!.AccessToken, TimeSpan.FromSeconds(tokenResult.ExpiresIn - 60));

            return tokenResult.AccessToken;

        }


        //public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        //{
        //    if (_cache.TryGetValue(TokenCacheKey, out string cachedToken))
        //    {
        //        return cachedToken;
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    client.BaseAddress = new Uri("https://zoom.us/");

        //    var request = new HttpRequestMessage(
        //        HttpMethod.Post,
        //        $"oauth/token?grant_type=account_credentials&account_id={_settings.AccountId}"
        //    );

        //    var credentials = Convert.ToBase64String(
        //        Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}")
        //    );

        //    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        //    // ✅ Required: Zoom expects application/x-www-form-urlencoded
        //    request.Content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded");

        //    var response = await client.SendAsync(request, ct);
        //    response.EnsureSuccessStatusCode();

        //    var json = await response.Content.ReadAsStringAsync(ct);

        //    var tokenResult = JsonSerializer.Deserialize<ZoomTokenResponse>(
        //        json,
        //        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //    );

        //    if (tokenResult == null || string.IsNullOrEmpty(tokenResult.AccessToken))
        //        throw new InvalidOperationException("Failed to retrieve Zoom access token.");

        //    // Cache token slightly less than expiry
        //    _cache.Set(TokenCacheKey, tokenResult.AccessToken, TimeSpan.FromSeconds(tokenResult.ExpiresIn - 60));

        //    _logger.LogInformation("Zoom access token cached for {ExpiresIn} seconds.", tokenResult.ExpiresIn);

        //    return tokenResult.AccessToken;
        //}


        //public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        //{
        //    if (_cache.TryGetValue(TokenCacheKey, out string cachedToken))
        //    {
        //        return cachedToken;
        //    }

        //    var client = _httpClientFactory.CreateClient();
        //    client.BaseAddress = new Uri("https://zoom.us/");

        //    // Zoom S2S OAuth token endpoint
        //    var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token?grant_type=account_credentials&account_id="
        //        + _settings.AccountId);
        //    var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
        //    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);


        //    var response = await client.SendAsync(request, ct);
        //    response.EnsureSuccessStatusCode();

        //    var json = await response.Content.ReadAsStringAsync(ct);

        //    var tokenResult = JsonSerializer.Deserialize<ZoomTokenResponse>(json, new JsonSerializerOptions 
        //    { PropertyNameCaseInsensitive = true });

        //    if (tokenResult == null || string.IsNullOrEmpty(tokenResult.AccessToken))
        //        throw new InvalidOperationException("Failed to retrieve Zoom access token.");

        //    // Cache token slightly less than expiry
        //    _cache.Set(TokenCacheKey, tokenResult.AccessToken, TimeSpan.FromSeconds(tokenResult.ExpiresIn - 60));

        //    _logger.LogInformation("Zoom access token cached for {ExpiresIn} seconds.", tokenResult.ExpiresIn);

        //    return tokenResult.AccessToken;
        //}


    }

}
