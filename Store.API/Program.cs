using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.API.MiddleWare;
using Store.Core.Interfaces;
using Store.Infrastructure.Repositories;
using Store.API.Services;
using Microsoft.EntityFrameworkCore;
using Store.Infrastructure.Data;

Env.NoClobber().TraversePath().Load();

// Configure application services before building the HTTP pipeline.
var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("StoreDb")))
    throw new InvalidOperationException(
            "Required configuration 'ConnectionStrings:StoreDb' is missing. Set the ConnectionStrings__StoreDb environment variable (for example in the repository-root .env file).");


// Convert MVC validation failures to the same response envelope used by middleware.
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
                        error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The supplied value is invalid." : error.ErrorMessage)))
                .ToArray();

            var response = ApiResponses.Failure(
                "Request validation failed.",
                errors,
                context.HttpContext.TraceIdentifier);

            return new BadRequestObjectResult(response);
        };
    });
// Expose the OpenAPI document and Swagger UI only in Development.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// DB Context
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("StoreDb")));

// Mapper
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Services 
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

// Add development-only API documentation endpoints.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Store API v1");
    });
}

// Handle exceptions and empty HTTP status responses with the shared API envelope.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages(async statusCodeContext =>
{
    var context = statusCodeContext.HttpContext;
    await context.Response.WriteAsJsonAsync(
        ApiResponses.FailureForStatusCode(context.Response.StatusCode, context.TraceIdentifier));
});

app.UseHttpsRedirection();

// Register controller routes after the shared HTTP middleware.
app.UseAuthorization();

app.MapControllers();

app.Run();
