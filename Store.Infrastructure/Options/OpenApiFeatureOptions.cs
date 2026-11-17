namespace Store.Infrastructure.Options;

/// <summary>Controls whether OpenAPI JSON and Swagger UI are served.</summary>
public sealed class OpenApiFeatureOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Swagger";

    /// <summary>When true, Swagger is served outside Development as well.</summary>
    public bool Enabled { get; set; }
}
