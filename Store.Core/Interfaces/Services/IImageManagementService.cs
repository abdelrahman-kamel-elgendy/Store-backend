using Microsoft.AspNetCore.Http;
using Store.Core.Common;

namespace Store.Core.Interfaces.Services;

/// <summary>Validates uploaded images and stores them on disk.</summary>
public interface IImageManagementService
{
    /// <summary>Validates and stores  images under the images root.</summary>
    /// <param name=" images"> images to store.</param>
    /// <param name="folderName">Folder segment derived from the product name.</param>
    Task<List<string>> AddImageAsync(IFormFileCollection images, string folderName);

    /// <summary>Deletes a previously stored relative path when the file exists.</summary>
    /// <param name="relativePath">Path stored on the image row.</param>
    Task DeleteImageAsync(string relativePath);
}
