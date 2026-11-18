using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Store.API.Helper;

namespace Store.API.MiddleWare;

/// <summary>
/// Middleware to append standard security headers to every HTTP response 
/// and enforce IP-based rate limiting.
/// Values are configurable via the .env file / environment variables.
/// </summary>
public sealed class SecurityHandlingMiddleware(RequestDelegate next, IMemoryCache memoryCache, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IConfiguration _configuration = configuration;

    // Load rate limit settings from .env or use defaults
    private readonly int _rateLimitMaxRequests = configuration.GetValue<int>("RATE_LIMIT_MAX_REQUESTS", 100);
    private readonly TimeSpan _rateLimitWindow = TimeSpan.FromSeconds(configuration.GetValue<int>("RATE_LIMIT_WINDOW_SECONDS", 60));

    public async Task InvokeAsync(HttpContext context)
    {
        ApplySecurityHeaders(context);

        if (!IsRequestAllowed(context))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = "application/json";

            var response = ApiResponses.Failure("Too many requests. Please try again later.", null, context.TraceIdentifier, StatusCodes.Status429TooManyRequests);

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await _next(context);
    }

    private bool IsRequestAllowed(HttpContext context)
    {
        // Safely get IP (fallback to loopback for local dev if null)
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var cacheKey = $"RateLimit:{ip}";
        var now = DateTime.UtcNow;

        if (!_memoryCache.TryGetValue(cacheKey, out RateLimitCounter? counter))
        {
            counter = new RateLimitCounter
            {
                Timestamp = now,
                Count = 1
            };
            _memoryCache.Set(cacheKey, counter, _rateLimitWindow);
            return true;
        }

        if (counter == null)
            return true;

        if (now - counter.Timestamp >= _rateLimitWindow)
        {
            counter.Timestamp = now;
            counter.Count = 1;
            _memoryCache.Set(cacheKey, counter, _rateLimitWindow);
            return true;
        }

        counter.Count++;
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
