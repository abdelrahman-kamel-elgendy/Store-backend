using System.Diagnostics;
using Store.API.Exceptions;
using Store.API.Helper;

namespace Store.API.MiddleWare;

/// <summary>converts application exceptions into standardized API error responses.</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment environment)
{

    /// <summary>invokes the next middleware and formats exceptions raised by the request.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug(
                "Request was canceled by the client. TraceId: {TraceId}",
                GetTraceId(context)
                );

            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (Exception exception)
        {
            var traceId = GetTraceId(context);

            if (context.Response.HasStarted)
            {
                logger.LogError(exception, "An exception occurred after the response started. TraceId: {TraceId}", traceId);
                throw;
            }

            await HandleExceptionAsync(context, exception, traceId);
        }
    }

    /// <summary>maps an exception to its status code, logs it, and writes the shared response.</summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        var result = MapException(exception);

        if (result.StatusCode >= StatusCodes.Status500InternalServerError)
            logger.LogError(
                exception,
                "Unhandled server error processing request. Status: {StatusCode}. TraceId: {TraceId}",
                result.StatusCode,
                traceId);

        else
            logger.LogWarning(
                "Request failed with status {StatusCode}: {Message}. TraceId: {TraceId}",
                result.StatusCode,
                result.Message,
                traceId);

        // if we are in Development, use the actual exception message for 500s to help debugging.
        var finalMessage = result.StatusCode == StatusCodes.Status500InternalServerError && environment.IsDevelopment() ? $"[DEV] {exception.Message}" : result.Message;

        // if in Development, we can also pass the StackTrace to the errors list
        var finalErrors = result.Errors;
        if (environment.IsDevelopment() && result.StatusCode >= 500)
            finalErrors = new List<ApiError> { new("StackTrace", exception.StackTrace ?? "No stack trace available") };


        var response = ApiResponses.Failure(
            finalMessage,
            finalErrors,
            traceId,
            result.StatusCode);

        context.Response.StatusCode = result.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>maps an exception to an HTTP status code, a client-safe message and optional field errors.</summary>
    private static ErrorResult MapException(Exception exception) => exception switch
    {
        ValidationException e => new(StatusCodes.Status400BadRequest, e.Message, [.. e.Errors.SelectMany(error => error.Value.Select(text => new ApiError(error.Key, text)))]),
        BadRequestException e => new(StatusCodes.Status400BadRequest, e.Message),
        NotFoundException e => new(StatusCodes.Status404NotFound, e.Message),
        UnauthorizedException e => new(StatusCodes.Status401Unauthorized, e.Message),
        ForbiddenException e => new(StatusCodes.Status403Forbidden, e.Message),
        ConflictException e => new(StatusCodes.Status409Conflict, e.Message),
        BusinessRuleException e => new(StatusCodes.Status422UnprocessableEntity, e.Message),
        RateLimitException e => new(StatusCodes.Status429TooManyRequests, e.Message),
        ServiceUnavailableException e => new(StatusCodes.Status503ServiceUnavailable, e.Message),

        // in production, we might NOT want to expose raw InvalidOperationException messages 
        InvalidOperationException e => new(StatusCodes.Status500InternalServerError, e.Message),

        // malformed request bodies, oversized payloads and similar errors raised by ASP.NET Core itself.
        BadHttpRequestException e => new(e.StatusCode, "The request is invalid."),

        // Never expose internal exception details to clients for unhandled exceptions.
        _ => new(StatusCodes.Status500InternalServerError, "An internal server error occurred. Please try again later.")
    };

    /// <summary>Returns the identifier shared by the log entry and the response so they can be correlated.</summary>
    private static string GetTraceId(HttpContext context) =>
        Activity.Current?.Id ?? context.TraceIdentifier;

    /// <summary>The status code, message and optional errors to return for an exception.</summary>
    private sealed record ErrorResult(int StatusCode, string Message, IReadOnlyList<ApiError>? Errors = null);
}