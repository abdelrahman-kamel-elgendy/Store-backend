using Store.Core.DTOs.Image;

namespace Store.Core.DTOs.Product;

/// <summary>A product returned by the API.</summary>
/// <param name="Id">Product id.</param>
/// <param name="Name">Display name.</param>
/// <param name="Description">Description.</param>
/// <param name="NewPrice">Current price.</param>
/// <param name="OldPrice">Previous price.</param>
/// <param name="CategoryName">Name of the related category, or an empty string when that category is not visible.</param>
/// <param name="ImageUrls">Images saved for the product.</param>
/// <param name="IsDeleted">True when the product is soft-deleted.</param>
/// <param name="CreatedAt">UTC creation time.</param>
/// <param name="UpdatedAt">UTC last-update time.</param>
/// <param name="DeletedAt">UTC deletion time, or null when the product is active.</param>
public record ProductResponseDto(
    int Id,
    string Name,
    string Description,
    decimal NewPrice,
    decimal OldPrice,
    string CategoryName,
    IReadOnlyList<ImageResponseDto> ImageUrls,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt) : IIdentifiableDto;
