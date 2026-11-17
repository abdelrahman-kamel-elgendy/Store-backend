namespace Store.Infrastructure.Options;

/// <summary>Browser origins allowed to call the API.</summary>
public sealed class CorsSettings
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Cors";

    /// <summary>Named CORS policy applied to the application.</summary>
    public const string PolicyName = "Default";

    /// <summary>Comma-separated origin list. Empty denies cross-origin browser calls.</summary>
    public string Origins { get; set; } = string.Empty;
}
