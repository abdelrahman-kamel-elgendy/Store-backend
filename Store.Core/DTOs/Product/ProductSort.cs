namespace Store.Core.DTOs.Product;

/// <summary>Sort orders available when listing products.</summary>
public enum ProductSort
{
    /// <summary>Order by id (default).</summary>
    Default = 0,
    PriceAsc,
    PriceDesc,
    NameAsc,
    NameDesc,
    Newest
}