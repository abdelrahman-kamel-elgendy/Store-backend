using Store.Infrastructure;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.API.MiddleWare;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("StoreDb")))
{
    throw new InvalidOperationException(
        "Required configuration 'ConnectionStrings:StoreDb' is missing. Set the ConnectionStrings__StoreDb environment variable (for example in the repository-root .env file).");
}

// Add services to the container.

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                    new ApiError(
                        string.IsNullOrWhiteSpace(entry.Key) ? null : entry.Key,
                        error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "The supplied value is invalid."
                            : error.ErrorMessage)))
                .ToArray();

            var response = ApiResponses.Failure(
                "Request validation failed.",
                errors,
                context.HttpContext.TraceIdentifier);

            return new BadRequestObjectResult(response);
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.InfrastructureConfig(builder.Configuration);
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Store API v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var context = statusCodeContext.HttpContext;
    await context.Response.WriteAsJsonAsync(
        ApiResponses.FailureForStatusCode(context.Response.StatusCode, context.TraceIdentifier),
        context.RequestAborted);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
