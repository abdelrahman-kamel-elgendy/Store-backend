using System.Diagnostics;
using Store.API.Errors;
using Store.API.Helper;

namespace Store.API.MiddleWare;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                logger.LogError(exception, "An exception occurred after the response started. TraceId: {TraceId}", context.TraceIdentifier);
                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                validationException.Message,
                validationException.Errors.SelectMany(error =>
                    error.Value.Select(message => new ApiError(error.Key, message))).ToArray()),
            BadRequestException badRequestException => (
                StatusCodes.Status400BadRequest,
                badRequestException.Message,
                (IReadOnlyList<ApiError>?)null),
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                notFoundException.Message,
                (IReadOnlyList<ApiError>?)null),
            UnauthorizedException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                unauthorizedException.Message,
                (IReadOnlyList<ApiError>?)null),
            ForbiddenException forbiddenException => (
                StatusCodes.Status403Forbidden,
                forbiddenException.Message,
                (IReadOnlyList<ApiError>?)null),
            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                conflictException.Message,
                (IReadOnlyList<ApiError>?)null),
            BusinessRuleException businessRuleException => (
                StatusCodes.Status422UnprocessableEntity,
                businessRuleException.Message,
                (IReadOnlyList<ApiError>?)null),
            RateLimitException rateLimitException => (
                StatusCodes.Status429TooManyRequests,
                rateLimitException.Message,
                (IReadOnlyList<ApiError>?)null),
            ServiceUnavailableException serviceUnavailableException => (
                StatusCodes.Status503ServiceUnavailable,
                serviceUnavailableException.Message,
                (IReadOnlyList<ApiError>?)null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An internal server error occurred. Please try again later.",
                (IReadOnlyList<ApiError>?)null)
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing request. TraceId: {TraceId}", context.TraceIdentifier);
        }
        else
        {
            logger.LogInformation(
                "Request failed with status {StatusCode}: {Message}. TraceId: {TraceId}",
                statusCode,
                message,
                context.TraceIdentifier);
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(
            ApiResponses.Failure(message, errors, Activity.Current?.Id ?? context.TraceIdentifier),
            context.RequestAborted);
    }
}
