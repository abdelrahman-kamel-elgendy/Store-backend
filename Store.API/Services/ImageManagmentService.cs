using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Store.Core.Exceptions;
using Store.Core.Interfaces.Services;
using Store.Core.Options;

namespace Store.Service.Services;

public class ImageManagmentService : IImageManagementService
{
    private readonly ImageStorageOptions _options;
    private readonly string _root;
    private readonly IFileProvider _fileProvider;

    public ImageManagmentService(IWebHostEnvironment env, IOptions<ImageStorageOptions> options, IFileProvider fileProvider)
    {
        _options = options.Value;
        _fileProvider = fileProvider;

        _root = Path.IsPathRooted(_options.RootPath) ? _options.RootPath : Path.Combine(env.ContentRootPath, _options.RootPath);
    }

    public async Task<List<string>> AddImageAsync(IFormFileCollection images, string src)
    {
        if (images is null || images.Count == 0)
            return [];

        if (images.Count > _options.MaxCount)
            throw new ValidationException($"At most {_options.MaxCount} images are allowed per request.");

        var directory = Path.Combine(_root, "Images", src);
        Directory.CreateDirectory(directory);

        var saved = new List<string>(images.Count);

        foreach (var file in images)
        {
            if (file.Length <= 0)
                continue;

            if (file.Length > _options.MaxBytes)
                throw new ValidationException($"'{file.FileName}' exceeds the {_options.MaxBytes / 1024 / 1024} MiB limit.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_options.AllowedExtensions.Contains(ext))
                throw new ValidationException($"Extension '{ext}' is not allowed. Allowed: {string.Join(", ", _options.AllowedExtensions)}.");

            if (!_options.AllowedContentTypes.Contains(file.ContentType))
                throw new ValidationException($"Content type '{file.ContentType}' is not allowed.");

            var name = $"{Guid.NewGuid():N}{ext}";
            var path = Path.Combine(directory, name);

            await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(stream);

            saved.Add($"Images/{src}/{name}");
        }

        return saved;
    }

    public Task DeleteImageAsync(string src)
    {
        var info = _fileProvider.GetFileInfo(src);
        if (info.PhysicalPath is null || !File.Exists(info.PhysicalPath))
            throw new NotFoundException($"File in src {src} not found.");

        File.Delete(info.PhysicalPath);
        return Task.CompletedTask;
    }
}