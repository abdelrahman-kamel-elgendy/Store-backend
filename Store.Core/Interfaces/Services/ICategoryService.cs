using Store.Core.DTOs.Category;

namespace Store.Core.Interfaces.Services;

/// <summary>Category use cases, including nested category trees.</summary>
public interface ICategoryService : ICRUDService<CategoryResponseDto, CategoryRequestDto>
{

}
