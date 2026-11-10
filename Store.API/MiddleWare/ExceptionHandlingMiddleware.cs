using System.Diagnostics;
using Store.API.Exceptions;
using Store.API.Helper;

namespace Store.API.MiddleWare;

/// <summary>Converts application exceptions into standardized API error responses.</summary>
public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>Non-standard status code (used by nginx) for requests the client abandoned.</summary>
    private const int ClientClosedRequest = 499;

    private const string InternalErrorMessage = "An internal server error occurred. Please try again later.";

    /// <summary>Invokes the next middleware and formats exceptions raised by the request.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client disconnected, so there is nobody left to send a response to.
            logger.LogDebug("Request was canceled by the client. TraceId: {TraceId}", GetTraceId(context));

            if (!context.Response.HasStarted)
                context.Response.StatusCode = ClientClosedRequest;
        }
        catch (Exception exception)
        {
            var traceId = GetTraceId(context);

            if (context.Response.HasStarted)
            {
                logger.LogError(
                    exception,
                    "An exception occurred after the response started. TraceId: {TraceId}",
                    traceId);
                throw;
            }

            await HandleExceptionAsync(context, exception, traceId);
        }
    }

    /// <summary>Maps an exception to its status code, logs it, and writes the shared response.</summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        var result = MapException(exception);

        if (result.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Unhandled exception processing request. Status: {StatusCode}. TraceId: {TraceId}",
                result.StatusCode,
                traceId);
        }
        else
        {
            logger.LogInformation(
                "Request failed with status {StatusCode}: {Message}. TraceId: {TraceId}",
                result.StatusCode,
                result.Message,
                traceId);
        }

        // The same status code goes in the HTTP response and in the JSON body.
        var response = ApiResponses.Failure(
            result.Message,
            result.Errors,
            traceId,
            result.StatusCode);

        context.Response.StatusCode = result.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    /// <summary>
    /// Maps an exception to an HTTP status code, a client-safe message and optional field errors.
    /// More specific exception types must come before their base types.
    /// </summary>
    private static ErrorResult MapException(Exception exception) => exception switch
    {
        ValidationException e => new(
            StatusCodes.Status400BadRequest,
            e.Message,
            [.. e.Errors.SelectMany(error => error.Value.Select(text => new ApiError(error.Key, text)))]),

        BadRequestException e => new(StatusCodes.Status400BadRequest, e.Message),
        NotFoundException e => new(StatusCodes.Status404NotFound, e.Message),
        UnauthorizedException e => new(StatusCodes.Status401Unauthorized, e.Message),
        ForbiddenException e => new(StatusCodes.Status403Forbidden, e.Message),
        ConflictException e => new(StatusCodes.Status409Conflict, e.Message),
        BusinessRuleException e => new(StatusCodes.Status422UnprocessableEntity, e.Message),
        RateLimitException e => new(StatusCodes.Status429TooManyRequests, e.Message),
        ServiceUnavailableException e => new(StatusCodes.Status503ServiceUnavailable, e.Message),
        InvalidOperationException e => new(StatusCodes.Status500InternalServerError, e.Message),

        // Malformed request bodies, oversized payloads and similar errors raised by ASP.NET Core itself.
        BadHttpRequestException e => new(e.StatusCode, "The request is invalid."),

        // Never expose internal exception details to clients.
        _ => new(StatusCodes.Status500InternalServerError, InternalErrorMessage)
    };

    /// <summary>Returns the identifier shared by the log entry and the response so they can be correlated.</summary>
    private static string GetTraceId(HttpContext context) =>
        Activity.Current?.Id ?? context.TraceIdentifier;

    /// <summary>The status code, message and optional errors to return for an exception.</summary>
    private sealed record ErrorResult(int StatusCode, string Message, IReadOnlyList<ApiError>? Errors = null);
}