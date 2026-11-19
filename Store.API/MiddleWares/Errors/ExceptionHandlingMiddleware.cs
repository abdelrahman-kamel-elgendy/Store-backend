using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Store.API.Services;

namespace Store.API.MiddleWares.Errors;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IApiResponseBuilder responseBuilder)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogDebug("Request was canceled by the client. TraceId: {TraceId}", GetTraceId(context));
            if (!context.Response.HasStarted)
                context.Response.StatusCode = 499;
        }
        catch (Exception exception)
        {
            var traceId = GetTraceId(context);

            if (context.Response.HasStarted)
                logger.LogError(exception, "Exception occurred after response started. TraceId: {TraceId}", traceId);

            var result = responseBuilder.BuildExceptionResponse(exception, traceId);

            if (result.StatusCode >= StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Server error. Status: {StatusCode}. TraceId: {TraceId}", result.StatusCode, traceId);
            else
                logger.LogWarning("Client error. Status: {StatusCode}. TraceId: {TraceId}", result.StatusCode, traceId);

            context.Response.StatusCode = result.StatusCode;
            await context.Response.WriteAsJsonAsync(result);
        }
    }

    private static string GetTraceId(HttpContext context) => Activity.Current?.Id ?? context.TraceIdentifier;
}