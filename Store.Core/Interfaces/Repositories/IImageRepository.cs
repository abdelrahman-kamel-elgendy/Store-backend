using Microsoft.AspNetCore.Http;

namespace Store.Core.Interfaces.Repositories;

public interface IImageRepository
{
    public Task SaveImagesAsync(IFormFileCollection images, int entityId, string entityName);
    public Task RemoveImagesAsync(int entityId);
}
