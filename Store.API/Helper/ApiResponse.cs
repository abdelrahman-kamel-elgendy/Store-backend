using System.Text.Json.Serialization;
namespace Store.API.Helper;

/// <summary>Standard envelope returned for JSON API responses.</summary>
/// <typeparam name="T">The response payload type.</typeparam>
/// <param name="Success">Whether the request completed successfully.</param>
/// <param name="StatusCode">The HTTP status code, repeated in the body for clients.</param>
/// <param name="Message">A human-readable summary of the result.</param>
/// <param name="Data">The response payload, or <c>null</c> when there is none.</param>
/// <param name="Errors">Field-level or general errors. Omitted from the JSON when <c>null</c>.</param>
/// <param name="Meta">Pagination and trace information. Omitted from the JSON when <c>null</c>.</param>
public sealed record ApiResponse<T>(
    bool Success,
    int StatusCode,
    string Message,
    T? Data,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<ApiError>? Errors = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] ApiMetadata? Meta = null);

/// <summary>Describes a single error, optionally tied to a request field.</summary>
/// <param name="Field">The related field name, or <c>null</c> for a general error.</param>
/// <param name="Message">The error message.</param>
public sealed record ApiError(
    string? Field,
    string Message);

/// <summary>Optional response metadata such as pagination and trace identifiers.</summary>
public sealed record ApiMetadata(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] PaginationMetadata? Pagination = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? TraceId = null);

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
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="page"/> or <paramref name="pageSize"/> is less than 1, or <paramref name="totalItems"/> is negative.
    /// </exception>
    public static PaginationMetadata Create(int page, int pageSize, int totalItems)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(totalItems);

        // Integer ceiling division avoids floating-point rounding.
        var totalPages = (int)(((long)totalItems + pageSize - 1) / pageSize);

        return new PaginationMetadata(page, pageSize, totalItems, totalPages, page > 1, page < totalPages);
    }
}

/// <summary>Creates the standard success and failure API response envelopes.</summary>
public static class ApiResponses
{
    /// <summary>Creates a successful response with optional metadata.</summary>
    public static ApiResponse<T> Success<T>(
        T? data,
        string message,
        ApiMetadata? meta = null,
        int statusCode = StatusCodes.Status200OK) =>
        new(true, statusCode, message, data, null, meta);

    /// <summary>
    /// Creates a failure response with optional errors and trace information.
    /// Pass the real <paramref name="statusCode"/> (for example 404 or 500); it defaults to 400.
    /// </summary>
    public static ApiResponse<object> Failure(
        string message,
        IReadOnlyList<ApiError>? errors = null,
        string? traceId = null,
        int statusCode = StatusCodes.Status400BadRequest) =>
        new(false, statusCode, message, null, errors,
            traceId is null ? null : new ApiMetadata(TraceId: traceId));

    /// <summary>Creates a standard failure response for an HTTP status code.</summary>
    public static ApiResponse<object> FailureForStatusCode(int statusCode, string? traceId = null)
    {
        var message = statusCode switch
        {
            StatusCodes.Status400BadRequest => "The request is invalid.",
            StatusCodes.Status401Unauthorized => "Authentication is required to access this resource.",
            StatusCodes.Status403Forbidden => "You are not authorized to access this resource.",
            StatusCodes.Status404NotFound => "The requested resource was not found.",
            StatusCodes.Status405MethodNotAllowed => "The HTTP method is not allowed for this resource.",
            StatusCodes.Status409Conflict => "The request conflicts with the current resource state.",
            StatusCodes.Status415UnsupportedMediaType => "The request content type is not supported.",
            StatusCodes.Status422UnprocessableEntity => "The request could not be processed.",
            StatusCodes.Status429TooManyRequests => "Too many requests. Please try again later.",
            StatusCodes.Status500InternalServerError => "An unexpected error occurred.",
            StatusCodes.Status503ServiceUnavailable => "The service is temporarily unavailable.",
            _ => "The request could not be completed."
        };

        return Failure(message, traceId: traceId, statusCode: statusCode);
    }

}