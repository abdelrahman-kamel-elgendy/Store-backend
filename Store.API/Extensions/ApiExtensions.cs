using Microsoft.AspNetCore.Mvc;
using Store.API.DTO;
using Store.API.Helper;

namespace Store.API.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value is { Errors.Count: > 0 })
                        .SelectMany(e => e.Value!.Errors.Select(error => new FieldValidationError(string.IsNullOrWhiteSpace(e.Key) ? null : e.Key, error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The supplied value is invalid." : error.ErrorMessage)))
                        .ToArray();

                    return new BadRequestObjectResult(ApiResponses.Failure(
                        "Request validation failed.", errors, context.HttpContext.TraceIdentifier));
                };
            });

        return services;
    }
}