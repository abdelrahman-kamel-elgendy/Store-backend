namespace Store.Core.DTOs.Image;

/// <summary>An image stored for a product.</summary>
/// <param name="Id">Image id.</param>
/// <param name="Url">Relative storage path, for example <c>Images/Headphones/file.jpg</c>.</param>
/// <param name="ProductId">Owning product id.</param>
/// <param name="IsMain">True when this image is the primary image. The API currently saves false.</param>
public record ImageResponseDto(
    int Id,
    string Url,
    int ProductId,
    bool IsMain) : IIdentifiableDto;
