namespace Store.Core.DTO.Category;

public record CategoryDto(
    int Id,
    string Name,
    string Description,
    int? ParentCategoryId,
    IReadOnlyList<CategoryDto> SubCategories) : IIdentifiableDto;
