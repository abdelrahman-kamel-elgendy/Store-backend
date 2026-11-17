using Store.Core.DTOs;
using Store.Core.DTOs.Product;

namespace Store.Core.Interfaces.Services;

/// <summary>Product use cases.</summary>
public interface IProductService : ICRUDService<ProductResponseDto, ProductRequestDto>
{
    Task<PagedResult<ProductResponseDto>> GetByCategoryIdAsync(int categoryId, int page, int pageSize, bool includeDeleted = false);
    Task<PagedResult<ProductResponseDto>> GetByCategoryNameAsync(string categoryName, int page, int pageSize, bool includeDeleted = false);
}
