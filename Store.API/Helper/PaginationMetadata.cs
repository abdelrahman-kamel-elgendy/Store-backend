namespace Store.API.Helper;

public sealed record PaginationMetadata(int Page, int PageSize, int TotalItems, int TotalPages, bool HasPreviousPage, bool HasNextPage)
{
    public static PaginationMetadata Create(int page, int pageSize, int totalItems)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems);

        var totalPages = (int)(((long)totalItems + pageSize - 1) / pageSize);

        return new PaginationMetadata(page, pageSize, totalItems, totalPages, page > 1, page < totalPages);
    }
}