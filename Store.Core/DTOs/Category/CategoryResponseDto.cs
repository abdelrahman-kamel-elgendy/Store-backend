namespace Store.Core.DTOs.Category;

/// <summary>A category and its nested children.</summary>
/// <param name="Id">Category id.</param>
/// <param name="Name">Display name.</param>
/// <param name="Description">Description.</param>
/// <param name="ParentCategoryId">Parent category id, or null for a root.</param>
/// <param name="SubCategories">Child categories, ordered according to the requested sort.</param>
/// <param name="IsDeleted">True when the category is soft-deleted.</param>
/// <param name="CreatedAt">UTC creation time.</param>
/// <param name="UpdatedAt">UTC last-update time.</param>
/// <param name="DeletedAt">UTC deletion time, or null when the category is active.</param>
public record CategoryResponseDto(
    int Id,
    string Name,
    string Description,
    int? ParentCategoryId,
    IReadOnlyList<CategoryResponseDto> SubCategories,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt) : IIdentifiableDto;
