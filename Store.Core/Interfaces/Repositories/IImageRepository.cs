using Microsoft.AspNetCore.Http;
using Store.Core.Common;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Stores and replaces product image rows and files.</summary>
public interface IImageRepository
{
    /// <summary>Saves image files and inserts their rows.</summary>
    /// <param name="images">Files to store. Null or empty is a no-op.</param>
    /// <param name="entityId">Owning product id.</param>
    /// <param name="entityName">Product name used as the storage folder.</param> 
    Task SaveImagesAsync(IFormFileCollection images, int entityId, string entityName);

    /// <summary>
    /// Replaces the image rows for a product. New files are written before the database change.
    /// Previously stored paths are returned so the caller can delete those files after the transaction commits.
    /// </summary>
    /// <param name="entityId">Owning product id.</param>
    /// <param name="entityName">Product name used as the storage folder.</param>
    /// <param name="images">Replacement files. Null or empty removes every image.</param> 
    /// <returns>Relative paths of the images that were replaced.</returns>
    Task<List<string>> ReplaceImagesAsync(int entityId, string entityName, IFormFileCollection? images);
}
