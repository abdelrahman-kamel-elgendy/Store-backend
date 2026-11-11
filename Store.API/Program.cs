using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Store.API.Helper;
using Store.API.MiddleWare;
using Store.Infrastructure.Repositories;
using Store.API.Services;
using Microsoft.EntityFrameworkCore;
using Store.Infrastructure.Data;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Servics;
using Store.Core.Interfaces.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Store.API.Configuration;
using Store.Infrastructure.Data.Seeding;

Env.NoClobber().TraversePath().Load();

// Configure application services before building the HTTP pipeline.
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("StoreDb");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Required configuration 'ConnectionStrings:StoreDb' is missing. Set ConnectionStrings__StoreDb in the repository-root .env file or the host environment.");

builder.Services
    .AddOptions<DatabaseOptions>()
    .Configure(options => options.ConnectionString = connectionString)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<SeedDatabaseOptions>()
    .Configure(options => options.Enabled = builder.Configuration.GetValue<bool>("SEED_DATABASE"))
    .Validate(
        options => !options.Enabled || builder.Environment.IsDevelopment(),
        "SEED_DATABASE=true is only allowed when ASPNETCORE_ENVIRONMENT=Development.")
    .ValidateOnStart();


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
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));

// Mapper
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());

// Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<DbSeeder>();

// Services 
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IImageManagementService, ImageManagmentService>();
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

var app = builder.Build();

if (app.Environment.IsDevelopment() &&
    app.Services.GetRequiredService<IOptions<SeedDatabaseOptions>>().Value.Enabled)
{
    await using var scope = app.Services.CreateAsyncScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

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
