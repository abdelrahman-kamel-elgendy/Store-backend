using System.ComponentModel.DataAnnotations;
using Store.Core.Common;

namespace Store.Core.DTOs.Product;

/// <summary>Criteria for listing products. Every property is optional.</summary>
/// <param name="CategoryId">When set, only products in this category are returned.</param>
/// <param name="Search">When set, only products whose name contains this text are returned.</param>
/// <param name="MinPrice">When set, only products with a new price at or above this value are returned.</param>
/// <param name="MaxPrice">When set, only products with a new price at or below this value are returned.</param>
/// <param name="Sort">
/// Sort order: <c>priceAsc</c>, <c>priceDesc</c>, <c>nameAsc</c>, <c>nameDesc</c> or <c>newest</c>.
/// When null, products are ordered by id.
/// </param>
/// <param name="IncludeDeleted">When true, soft-deleted products are included.</param>
public sealed record ProductFilter(
    [Range(1, int.MaxValue, ErrorMessage = "Category id must be greater than zero.")]
    int? CategoryId = null,

    [StringLength(100, ErrorMessage = "Search text cannot be longer than 100 characters.")]
    string? Search = null,

    [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "Minimum price must be between 0 and 9999999.99.")]
    decimal? MinPrice = null,

    [Range(typeof(decimal), "0", "9999999.99", ErrorMessage = "Maximum price must be between 0 and 9999999.99.")]
    decimal? MaxPrice = null,

    [EnumDataType(typeof(ProductSort), ErrorMessage = "Sort must be one of: PriceAsc, PriceDesc, NameAsc, NameDesc, Newest.")]
    ProductSort Sort = ProductSort.Default,

    bool IncludeDeleted = false) : IFilter, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice.HasValue && MaxPrice.HasValue && MinPrice > MaxPrice)
            yield return new ValidationResult("Minimum price cannot be greater than maximum price.", [nameof(MinPrice), nameof(MaxPrice)]);

    }
}