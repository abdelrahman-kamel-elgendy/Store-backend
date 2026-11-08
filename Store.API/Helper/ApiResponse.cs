using Microsoft.AspNetCore.Http;

namespace Store.API.Helper;

/// <summary>Standard envelope returned for JSON API responses.</summary>
/// <typeparam name="T">The response payload type.</typeparam>
public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyList<ApiError>? Errors = null,
    ApiMetadata? Meta = null);

/// <summary>A field-specific or general API error message.</summary>
public sealed record ApiError(string? Field, string Message);

/// <summary>Optional response metadata such as pagination and trace identifiers.</summary>
public sealed record ApiMetadata(
    PaginationMetadata? Pagination = null,
    string? TraceId = null);

/// <summary>Describes the current page and the available navigation directions.</summary>
public sealed record PaginationMetadata(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    /// <summary>Creates pagination metadata from the page request and total result count.</summary>
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

/// <summary>Creates the standard success and failure API response envelopes.</summary>
public static class ApiResponses
{
    /// <summary>Creates a successful response with optional metadata.</summary>
    public static ApiResponse<T> Success<T>(
        T? data,
        string message,
        ApiMetadata? meta = null) =>
        new(true, message, data, null, meta);

    /// <summary>Creates a successful response with no payload.</summary>
    public static ApiResponse<object> Success(string message) =>
        Success<object>(null, message);

    /// <summary>Creates a failure response with optional errors and trace information.</summary>
    public static ApiResponse<object> Failure(
        string message,
        IReadOnlyList<ApiError>? errors = null,
        string? traceId = null) =>
        new(false, message, null, errors, traceId is null ? null : new ApiMetadata(TraceId: traceId));

    /// <summary>Creates a standard failure message for an HTTP status code.</summary>
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
