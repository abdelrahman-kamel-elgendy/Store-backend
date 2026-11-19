using System.ComponentModel.DataAnnotations;

namespace Store.Core.Options;

public sealed class ImageStorageOptions
{
    public const string SectionName = "Images";

    [Required]
    public string RootPath { get; set; } = "wwwroot";

    [Range(1, long.MaxValue)]
    public long MaxBytes { get; set; } = 5 * 1024 * 1024;

    [Range(1, 50)]
    public int MaxCount { get; set; } = 5;

    [MinLength(1)]
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    /// <summary>Content types accepted.</summary>
    [MinLength(1)]
    public string[] AllowedContentTypes { get; set; } = ["image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif"];
}
