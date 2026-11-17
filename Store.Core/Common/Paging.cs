using Store.Core.Exceptions;

namespace Store.Core.Common;

/// <summary>Guards page numbers shared by services.</summary>
public static class Paging
{
    /// <summary>Largest page size accepted by list endpoints.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Throws when the page request is outside the supported range.</summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Requested page size.</param>
    /// <exception cref="BadRequestException">The page or page size is out of range.</exception>
    public static void EnsureValid(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
            throw new BadRequestException($"Page must be at least 1 and page size must be between 1 and {MaxPageSize}.");
    }

    /// <summary>Converts a page number to a non-negative skip that fits in an integer.</summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>The number of items to skip.</returns>
    public static int Skip(int page, int pageSize) =>
        (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue);
}
