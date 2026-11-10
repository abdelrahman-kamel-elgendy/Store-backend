using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Store.API.Helper;

/// <summary>Non-generic view of an API response, used to read the status code without knowing the payload type.</summary>
public interface IApiResponse
{
    /// <summary>The HTTP status code of the response.</summary>
    int StatusCode { get; }
}

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
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] ApiMetadata? Meta = null) : IApiResponse;

/// <summary>Describes a single error, optionally tied to a request field.</summary>
/// <param name="Field">The related field name, or <c>null</c> for a general error.</param>
/// <param name="Message">The error message.</param>
/// <param name="Code">An optional machine-readable error code (for example, "NAME_REQUIRED").</param>
public sealed record ApiError(
    string? Field,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Code = null);

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
    private const string ValidationMessage = "One or more validation errors occurred.";
    private const string InvalidValueMessage = "The value is invalid.";

    // ---------- Success ----------

    /// <summary>Creates a successful response with optional metadata.</summary>
    public static ApiResponse<T> Success<T>(
        T? data,
        string message,
        ApiMetadata? meta = null,
        int statusCode = StatusCodes.Status200OK) =>
        new(true, statusCode, message, data, null, meta);

    /// <summary>Creates a successful response with no payload.</summary>
    public static ApiResponse<object> Success(string message, int statusCode = StatusCodes.Status200OK) =>
        Success<object>(null, message, null, statusCode);

    /// <summary>Creates a 201 Created response for a newly created resource.</summary>
    public static ApiResponse<T> Created<T>(T data, string message = "Resource created successfully.", string? traceId = null) =>
        Success(data, message, traceId is null ? null : new ApiMetadata(TraceId: traceId), StatusCodes.Status201Created);

    /// <summary>Creates a successful paged response with pagination metadata.</summary>
    /// <param name="items">The items on the current page.</param>
    /// <param name="page">The current 1-based page number.</param>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="totalItems">The total number of items across all pages.</param>
    /// <param name="message">The response message.</param>
    /// <param name="traceId">An optional trace identifier.</param>
    public static ApiResponse<IReadOnlyList<T>> Paged<T>(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalItems,
        string message = "Data retrieved successfully.",
        string? traceId = null)
    {
        ArgumentNullException.ThrowIfNull(items);

        var meta = new ApiMetadata(PaginationMetadata.Create(page, pageSize, totalItems), traceId);
        return Success<IReadOnlyList<T>>(items, message, meta);
    }

    // ---------- Failure ----------

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

    /// <summary>Creates a failure response for a list of validation errors (400 by default).</summary>
    public static ApiResponse<object> ValidationFailure(
        IReadOnlyList<ApiError> errors,
        string? traceId = null,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return Failure(ValidationMessage, errors, traceId, statusCode);
    }

    /// <summary>Creates a validation failure from the invalid entries of a <see cref="ModelStateDictionary"/>.</summary>
    public static ApiResponse<object> ValidationFailure(
        ModelStateDictionary modelState,
        string? traceId = null,
        int statusCode = StatusCodes.Status400BadRequest)
    {
        ArgumentNullException.ThrowIfNull(modelState);

        var errors = modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .SelectMany(entry => entry.Value!.Errors.Select(error => new ApiError(
                string.IsNullOrEmpty(entry.Key) ? null : entry.Key,
                string.IsNullOrWhiteSpace(error.ErrorMessage) ? InvalidValueMessage : error.ErrorMessage)))
            .ToList();

        return Failure(ValidationMessage, errors, traceId, statusCode);
    }

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

    // ---------- Controller helper ----------

    /// <summary>
    /// Converts a response into an <see cref="ObjectResult"/> whose HTTP status matches the
    /// <c>statusCode</c> in the body, so the two can never disagree.
    /// </summary>
    public static ObjectResult ToActionResult(this IApiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return new ObjectResult(response) { StatusCode = response.StatusCode };
    }
}