using Store.API.Helper;
using Store.Core.Exceptions;

namespace Store.API.Services;

public sealed class ApiResponseBuilder(IWebHostEnvironment environment) : IApiResponseBuilder
{
    public ApiResponse<object> BuildExceptionResponse(Exception exception, string traceId)
    {
        var (statusCode, message, errors) = MapException(exception);

        // Environment-aware logic for 500-level errors
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            if (environment.IsDevelopment())
            {
                message = $"[DEV] {exception.Message}";
                errors = new List<ApiError> { new("StackTrace", exception.StackTrace ?? "No stack trace available") };
            }
            else if (environment.IsStaging())
            {
                message = $"[STAGING] {exception.Message}";
            }
            else
            {
                message = "An internal server error occurred. Please try again later.";
            }
        }

        return ApiResponses.Failure(message, errors, traceId, statusCode);
    }

    public ApiResponse<T> BuildSuccessResponse<T>(T data, string message, ApiMetadata? meta = null)
        => ApiResponses.Success(data, message, meta);

    public ApiResponse<object> BuildStatusCodeResponse(int statusCode, string traceId)
        => ApiResponses.FailureForStatusCode(statusCode, traceId);

    private static (int StatusCode, string Message, IReadOnlyList<ApiError>? Errors) MapException(Exception exception) => exception switch
    {
        ValidationsException e => (StatusCodes.Status400BadRequest, e.Message, e.Errors.SelectMany(x => x.Value.Select(t => new ApiError(x.Key, t))).ToList()),
        BadRequestException e => (StatusCodes.Status400BadRequest, e.Message, null),
        NotFoundException e => (StatusCodes.Status404NotFound, e.Message, null),
        UnauthorizedException e => (StatusCodes.Status401Unauthorized, e.Message, null),
        ForbiddenException e => (StatusCodes.Status403Forbidden, e.Message, null),
        ConflictException e => (StatusCodes.Status409Conflict, e.Message, null),
        BusinessRuleException e => (StatusCodes.Status422UnprocessableEntity, e.Message, null),
        RateLimitException e => (StatusCodes.Status429TooManyRequests, e.Message, null),
        ServiceUnavailableException e => (StatusCodes.Status503ServiceUnavailable, e.Message, null),
        InvalidOperationException e => (StatusCodes.Status500InternalServerError, e.Message, null),
        BadHttpRequestException e => (e.StatusCode, "The request is invalid.", null),
        _ => (StatusCodes.Status500InternalServerError, "An internal server error occurred.", null)
    };
}