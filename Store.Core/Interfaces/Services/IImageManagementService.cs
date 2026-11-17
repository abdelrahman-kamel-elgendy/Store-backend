using Store.Core.Common;

namespace Store.Core.Interfaces.Services;

/// <summary>Validates uploaded images and stores them on disk.</summary>
public interface IImageManagementService
{
    /// <summary>Checks count, size, extension, and content type without writing files.</summary>
    /// <param name="files">Files to check. Null or empty is valid.</param>
    void Validate(IReadOnlyList<UploadedFile>? files);

    /// <summary>Validates and stores files under the images root.</summary>
    /// <param name="files">Files to store.</param>
    /// <param name="folderName">Folder segment derived from the product name.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Relative paths that should be stored on image rows.</returns>
    Task<IReadOnlyList<string>> AddImagesAsync(IReadOnlyList<UploadedFile>? files, string folderName, CancellationToken cancellationToken = default);

    /// <summary>Deletes a previously stored relative path when the file exists.</summary>
    /// <param name="relativePath">Path stored on the image row.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task DeleteImageAsync(string relativePath, CancellationToken cancellationToken = default);
}
