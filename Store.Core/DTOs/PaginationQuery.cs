using System.ComponentModel.DataAnnotations;
using Store.Core.Common;

namespace Store.Core.DTOs;

/// <summary>Query string accepted by collection endpoints.</summary>
public sealed record PaginationQuery
{
    /// <summary>1-based page number. Defaults to 1.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; init; } = 1;

    /// <summary>Page size. Defaults to 20 and cannot exceed 100.</summary>
    /// <example>20</example>
    [Range(1, Paging.MaxPageSize, ErrorMessage = "Page size must be between 1 and 100.")]
    public int PageSize { get; init; } = 20;

    /// <summary>When true, soft-deleted resources are included. Defaults to false.</summary>
    /// <example>false</example>
    public bool IncludeDeleted { get; init; }

    /// <summary>Optional case-insensitive match against the resource name.</summary>
    /// <example>audio</example>
    [StringLength(100)]
    public string? Search { get; init; }

    /// <summary>
    /// Optional sort key: id, -id, name, -name, createdAt, -createdAt.
    /// Products also accept newPrice, -newPrice, oldPrice, and -oldPrice.
    /// </summary>
    /// <example>name</example>
    [StringLength(32)]
    public string? Sort { get; init; }

    /// <summary>Copies these query values into the service-layer query object.</summary>
    /// <returns>A <see cref="ListQuery"/> with the same values.</returns>
    public ListQuery ToListQuery() => new(Page, PageSize, IncludeDeleted, Search, Sort);
}
