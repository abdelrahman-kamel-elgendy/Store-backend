using System;
using System.ComponentModel.DataAnnotations;

namespace Store.Core.Options;

public sealed class CorsSettings
{
    public const string SectionName = "Cors";

    [Required]
    public string[] AllowedOrigins { get; set; } = [];

    public bool AllowAnyHeader { get; set; } = true;
    public bool AllowAnyMethod { get; set; } = true;
}
