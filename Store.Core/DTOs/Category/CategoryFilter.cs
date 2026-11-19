using System.ComponentModel.DataAnnotations;

namespace Store.Core.DTOs.Category;

/// <summary>Criteria for listing categories. Every property is optional.</summary>
/// <param name="Search">
/// When set, only root categories that match, or that contain a matching subcategory, are returned.
/// Matching is a case-insensitive "name contains" check.
/// </param>
/// <param name="Sort">Sort order for root categories. Defaults to <c>Default</c> (by id).</param>
/// <param name="IncludeDeleted">When true, soft-deleted categories are included.</param>
public sealed record CategoryFilter(
    [StringLength(100, ErrorMessage = "Search text cannot be longer than 100 characters.")]
    string? Search = null,

    [EnumDataType(typeof(CategorySort), ErrorMessage = "Sort must be one of: NameAsc, NameDesc, Newest.")]
    CategorySort Sort = CategorySort.Default,

    bool IncludeDeleted = false) : IFilter;