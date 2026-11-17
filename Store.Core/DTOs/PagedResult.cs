namespace Store.Core.DTOs;

/// <summary>A page of results and the values needed to calculate pagination metadata.</summary>
/// <typeparam name="T">The type of each item in the page.</typeparam>
/// <param name="Items">The items in this page.</param>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Requested page size.</param>
/// <param name="TotalItems">Total number of matching items.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems);
