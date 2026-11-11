using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Store.Core.Entities;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Infrastructure.Data;

namespace Store.Infrastructure.Repositories;

public class ImageRepository(AppDbContext context, IImageManagementService imageManagementService) : IImageRepository
{
    private readonly AppDbContext _context = context;
    private readonly IImageManagementService _imageManagementService = imageManagementService;


    public async Task SaveImagesAsync(IFormFileCollection images, int entityId, string entityName)
    {
        var imagePaths = await _imageManagementService.AddImageAsync(images, entityName);
        var image = imagePaths.Select(path => new Image { Url = path, ProductId = entityId, }).ToList();

        await _context.Images.AddRangeAsync(image);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveImagesAsync(int entityId)
    {
        var images = await _context.Images.Where(image => image.ProductId == entityId).ToListAsync();

        foreach (var image in images)
            await _imageManagementService.DeleteImageAsync(image.Url);

        _context.Images.RemoveRange(images);
    }
}
