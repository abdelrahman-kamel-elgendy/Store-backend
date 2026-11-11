using Microsoft.Extensions.FileProviders;
using Store.API.Exceptions;
using Store.Core.Interfaces.Services;

namespace Store.API.Services;

public class ImageManagmentService(IFileProvider fileProvider) : IImageManagementService
{
    private readonly IFileProvider fileProvider = fileProvider;



    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        var savedImages = new List<string>();
        var imageDirectory = Path.Combine("wwwroot", "Images", src);

        Directory.CreateDirectory(imageDirectory);

        foreach (var file in files)
        {
            if (file.Length <= 0)
                continue;

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(imageDirectory, imageName);

            await file.CopyToAsync(new FileStream(filePath, FileMode.CreateNew));
            savedImages.Add($"Images/{src}/{imageName}");
        }

        return savedImages;
    }


    public async Task DeleteImageAsync(string src) => await Task.Run(() =>
            File.Delete(fileProvider.GetFileInfo(src).PhysicalPath ?? throw new NotFoundException($"File in src {src} not found.")));
}
