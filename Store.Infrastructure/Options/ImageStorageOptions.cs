using System.ComponentModel.DataAnnotations;

namespace Store.Infrastructure.Options;

/// <summary>Limits and location for product image files.</summary>
public sealed class ImageStorageOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Images";

    /// <summary>Directory that contains the Images folder. Relative paths are based on the content root.</summary>
    [Required]
    public string RootPath { get; set; } = "wwwroot";

    /// <summary>Maximum size of one file, in bytes. Defaults to 5 MiB.</summary>
    [Range(1, long.MaxValue)]
    public long MaxBytes { get; set; } = 5 * 1024 * 1024;

    /// <summary>Maximum number of files in one request.</summary>
    [Range(1, 50)]
    public int MaxCount { get; set; } = 8;

    /// <summary>Extensions accepted, including the leading dot.</summary>
    [MinLength(1)]
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    /// <summary>Content types accepted.</summary>
    [MinLength(1)]
    public string[] AllowedContentTypes { get; set; } = ["image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"];
}
