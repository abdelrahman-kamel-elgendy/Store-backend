using Store.Core.Entities;

namespace Store.Core.Interfaces.Repositories;

/// <summary>Product persistence. Reads include the category and images.</summary>
public interface IProductRepository : ICRUDRepository<Product>
{
    /// <summary>returns products by category id.</summary>
    /// <param name="categoryId">category id.</param>
    /// <param name="skip">Number of matching rows to skip.</param>
    /// <param name="take">Number of rows to return.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, int skip, int take, bool includeDeleted = false);

    /// <summary>returns products by category name.</summary>
    /// <param name="categoryName">category name.</param>
    /// 
    /// <param name="skip">Number of matching rows to skip.</param>
    /// <param name="take">Number of rows to return.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are included.</param>
    Task<IReadOnlyList<Product>> GetByCategoryNameAsync(string categoryName, int skip, int take, bool includeDeleted = false);

    /// <summary>Counts products spacific in category.</summary>
    /// <param name="categoryId">category id.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are counted.</param>
    Task<int> CountByCategoryAsync(int categoryId, bool includeDeleted = false);
    /// <summary>Counts products spacific in category.</summary>
    /// <param name="categoryName">category id.</param>
    /// <param name="includeDeleted">When true, soft-deleted rows are counted.</param>
    Task<int> CountByCategoryAsync(string categoryName, bool includeDeleted = false);
}
