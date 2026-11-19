using Store.Core.DTOs;
using Store.Core.DTOs.Product;

namespace Store.Core.Interfaces.Services;

/// <summary>Product use cases.</summary>
public interface IProductService : ICRUDService<ProductResponseDto, ProductRequestDto>
{

    /// <summary>Returns one page of products that match the filter.</summary>
    /// <param name="categoryId">When has value, only products in spacific category are returned.</param>
    /// <param name="skip">Number of matching rows to skip.</param>
    /// <param name="take">Number of rows to return.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<PagedResult<ProductResponseDto>> GetPageAsync(int? categoryId, int skip, int take, bool includeDeleted = false);
}
