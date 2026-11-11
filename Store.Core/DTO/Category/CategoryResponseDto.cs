namespace Store.Core.DTO.Category;

public record CategoryResponseDto(
    int Id,
    string Name,
    string Description,
    int? ParentCategoryId,
    IReadOnlyList<CategoryResponseDto> SubCategories,
    bool IsDelted,
    DateTime CreatdAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt
    ) : IIdentifiableDto;
