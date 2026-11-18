using Microsoft.Extensions.Caching.Memory;
using Store.API.Helper;
using Store.API.Services;
using Microsoft.AspNetCore.Http;

namespace Store.API.MiddleWares.Security;

public sealed class SecurityHandlingMiddleware(
    RequestDelegate next,
    IMemoryCache memoryCache,
    IConfiguration configuration,
    IApiResponseBuilder responseBuilder) // Inject Builder
{
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IConfiguration _configuration = configuration;
    private readonly IApiResponseBuilder _responseBuilder = responseBuilder;

    private readonly int _rateLimitMaxRequests = configuration.GetValue<int>("RATE_LIMIT_MAX_REQUESTS", 100);
    private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(configuration.GetValue<int>("RATE_LIMIT_WINDOW_SECONDS", 60));

    public async Task InvokeAsync(HttpContext context)
    {
        ApplySecurityHeaders(context);

        if (!IsRequestAllowed(context))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = _responseBuilder.BuildStatusCodeResponse(StatusCodes.Status429TooManyRequests, context.TraceIdentifier);
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }

    private bool IsRequestAllowed(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"RateLimit:{ip}";
        var now = DateTime.UtcNow;

        if (!_memoryCache.TryGetValue(cacheKey, out RateLimitCounter? counter))
        {
            _memoryCache.Set(cacheKey, new RateLimitCounter { Timestamp = now, Count = 1 }, _rateLimitWindow);
            return true;
        }

        if (counter == null) return true;

        if (now - counter.Timestamp >= _rateLimitWindow)
        {
            counter.Timestamp = now;
            counter.Count = 1;
        }
        else
        {
            counter.Count++;
        }

        _memoryCache.Set(cacheKey, counter, _rateLimitWindow);
        return counter.Count <= _rateLimitMaxRequests;
    }

    private void ApplySecurityHeaders(HttpContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = _configuration["SECURITY_CONTENT_TYPE_OPTIONS"] ?? "nosniff";
        context.Response.Headers["X-XSS-Protection"] = _configuration["SECURITY_XSS_PROTECTION"] ?? "1; mode=block";
        context.Response.Headers["X-Frame-Options"] = _configuration["SECURITY_FRAME_OPTIONS"] ?? "DENY";
        context.Response.Headers["Referrer-Policy"] = _configuration["SECURITY_REFERRER_POLICY"] ?? "strict-origin-when-cross-origin";

        if (context.Request.IsHttps)
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";

        var csp = _configuration["SECURITY_CSP"];
        if (!string.IsNullOrWhiteSpace(csp))
            context.Response.Headers["Content-Security-Policy"] = csp;
    }
}