using System.ComponentModel.DataAnnotations;

namespace Store.Infrastructure.Options;

/// <summary>Fixed-window limit applied to API controllers.</summary>
public sealed class RateLimitOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "RateLimiting";

    /// <summary>Requests allowed per window for each client IP.</summary>
    [Range(1, 100_000)]
    public int PermitLimit { get; set; } = 100;

    /// <summary>Window length in seconds.</summary>
    [Range(1, 86_400)]
    public int WindowSeconds { get; set; } = 60;
}
