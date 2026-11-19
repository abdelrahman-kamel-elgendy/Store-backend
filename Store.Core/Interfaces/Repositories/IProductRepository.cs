using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Product persistence. Reads include the category and images.</summary>
public interface IProductRepository : ICRUDRepository<Product>
{

    /// <summary>Returns one page of products that match the filter.</summary>
    /// <param name="categoryId">When has value, only products in spacific category are returned.</param>
    /// <param name="skip">Number of matching rows to skip.</param>
    /// <param name="take">Number of rows to return.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<IReadOnlyList<Product>> GetPageAsync(int? categoryId, int skip, int take, bool includeDeleted = false);

    /// <summary>Counts products that match the filter.</summary>
    /// <param name="categoryId">When has value, only products in spacific category are counted.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are counted.</param>
    Task<int> CountAsync(int? categoryId, bool includeDeleted = false);

}
