namespace Store.Core.DTO;

/// <summary>A page of results and the values needed to calculate pagination metadata.</summary>
/// <typeparam name="T">The type of each item in the page.</typeparam>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems);
