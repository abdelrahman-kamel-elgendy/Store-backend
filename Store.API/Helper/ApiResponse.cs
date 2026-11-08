using Microsoft.AspNetCore.Http;

namespace Store.API.Helper;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyList<ApiError>? Errors = null,
    ApiMetadata? Meta = null);

public sealed record ApiError(string? Field, string Message);

public sealed record ApiMetadata(
    PaginationMetadata? Pagination = null,
    string? TraceId = null);

public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    public static PaginationMetadata Create(int page, int pageSize, int totalItems)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems);

        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling((double)totalItems / pageSize);

        return new PaginationMetadata(
            page,
            pageSize,
            totalItems,
            totalPages,
            page > 1,
            page < totalPages);
    }
}

public static class ApiResponses
{
    public static ApiResponse<T> Success<T>(
        T? data,
        string message,
        ApiMetadata? meta = null) =>
        new(true, message, data, null, meta);

    public static ApiResponse<object> Success(string message) =>
        Success<object>(null, message);

    public static ApiResponse<object> Failure(
        string message,
        IReadOnlyList<ApiError>? errors = null,
        string? traceId = null) =>
        new(false, message, null, errors, traceId is null ? null : new ApiMetadata(TraceId: traceId));

    public static ApiResponse<object> FailureForStatusCode(int statusCode, string? traceId = null)
    {
        var message = statusCode switch
        {
            StatusCodes.Status400BadRequest => "The request is invalid.",
            StatusCodes.Status401Unauthorized => "Authentication is required to access this resource.",
            StatusCodes.Status403Forbidden => "You are not authorized to access this resource.",
            StatusCodes.Status404NotFound => "The requested resource was not found.",
            StatusCodes.Status409Conflict => "The request conflicts with the current resource state.",
            StatusCodes.Status422UnprocessableEntity => "The request could not be processed.",
            StatusCodes.Status429TooManyRequests => "Too many requests. Please try again later.",
            StatusCodes.Status503ServiceUnavailable => "The service is temporarily unavailable.",
            _ => "The request could not be completed."
        };

        return Failure(message, traceId: traceId);
    }
}
