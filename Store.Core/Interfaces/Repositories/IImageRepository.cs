using Microsoft.AspNetCore.Http;
using Store.Core.Common;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Stores and replaces  entity image rows and files.</summary>
public interface IImageRepository
{
    /// <summary>Saves image files and inserts their rows.</summary>
    /// <param name="images">Files to store. Null or empty is a no-op.</param>
    /// <param name="entityId">Owning  entity id.</param>
    /// <param name="entityName"> entity name used as the storage folder.</param> 
    Task SaveImagesAsync(IFormFileCollection images, int entityId, string entityName);

    /// <summary>REmoves image files and inserts their rows.</summary>
    /// <param name="entityId">Owning  entity id.</param>
    Task RemoveImagesAsync(int entityId);
}
