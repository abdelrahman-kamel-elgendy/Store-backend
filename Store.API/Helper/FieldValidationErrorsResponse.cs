using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Store.API.Helper;

public sealed record ApiResponse<T>(
    bool Success,
    int StatusCode,
    string Message,
    T? Data,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<ApiError>? Errors = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] ApiMetadata? Meta = null);

public sealed record ApiError(string? Field, string Message);

public sealed record ApiMetadata(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] PaginationMetadata? Pagination = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? TraceId = null);

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

public static class ApiResponses
{
    private static readonly Dictionary<int, string> StatusCodeMessages = new()
    {
        [StatusCodes.Status400BadRequest] = "The request is invalid.",
        [StatusCodes.Status401Unauthorized] = "Authentication is required.",
        [StatusCodes.Status403Forbidden] = "You are not authorized.",
        [StatusCodes.Status404NotFound] = "The requested resource was not found.",
        [StatusCodes.Status405MethodNotAllowed] = "Method not allowed.",
        [StatusCodes.Status409Conflict] = "Conflict with the current resource state.",
        [StatusCodes.Status415UnsupportedMediaType] = "Unsupported content type.",
        [StatusCodes.Status422UnprocessableEntity] = "The request could not be processed.",
        [StatusCodes.Status429TooManyRequests] = "Too many requests.",
        [StatusCodes.Status500InternalServerError] = "An unexpected error occurred.",
        [StatusCodes.Status503ServiceUnavailable] = "Service temporarily unavailable."
    };

    public static ApiResponse<T> Success<T>(T? data, string message, ApiMetadata? meta = null, int statusCode = StatusCodes.Status200OK)
        => new(true, statusCode, message, data, null, meta);

    public static ApiResponse<object> Failure(string message, IReadOnlyList<ApiError>? errors = null, string? traceId = null, int statusCode = StatusCodes.Status400BadRequest)
        => new(false, statusCode, message, null, errors, traceId is null ? null : new ApiMetadata(TraceId: traceId));

    public static ApiResponse<object> FailureForStatusCode(int statusCode, string? traceId = null)
        => Failure(StatusCodeMessages.GetValueOrDefault(statusCode, "The request could not be completed."), traceId: traceId, statusCode: statusCode);
}