namespace Store.Core.Common;

/// <summary>Paging, inclusion, search, and sort values for a collection query.</summary>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Number of items in the page. Maximum 100.</param>
/// <param name="IncludeDeleted">When true, soft-deleted rows are included.</param>
/// <param name="Search">Optional case-insensitive name filter.</param>
/// <param name="Sort">Optional sort key. See <see cref="QuerySort"/>.</param>
public sealed record ListQuery(int Page, int PageSize, bool IncludeDeleted, string? Search, string? Sort);
