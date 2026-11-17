using Microsoft.EntityFrameworkCore;
using Store.API.Configuration;
using Store.API.Helper;
using Store.API.MiddleWare;
using Store.Infrastructure.Data;
using Store.Infrastructure.Data.Seed;

namespace Store.API.Extensions;

/// <summary>
/// Extension methods for configuring the <see cref="WebApplication"/> pipeline
/// and running one-off startup tasks (e.g. database migrations and seeding).
/// </summary>
public static class WebApplicationExtensions
{
    //  middleware
    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        // catches exceptions thrown anywhere further down the pipeline.
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // ensures responses produced without a body (e.g. 401, 403, 404)
        app.UseStatusCodePages(async ctx =>
        {
            var http = ctx.HttpContext;
            await http.Response.WriteAsJsonAsync(ApiResponses.FailureForStatusCode(http.Response.StatusCode, http.TraceIdentifier));
        });

        return app;
    }

    //  seeding
    public static async Task<WebApplication> UseSeedingAsync(this WebApplication app)
    {
        // AppDbContext is scoped; resolve it from a new scope.
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var db = services.GetRequiredService<AppDbContext>();

        // Ensure the database schema is up to date before seeding.
        await db.Database.MigrateAsync();

        // Seeders
        await CategorySeeder.SeedAsync(db);
        await ProductSeeder.SeedAsync(db);

        return app;
    }

    public static async Task<bool> RunSeedingIfRequestedAsync(this WebApplication app, string[] args)
    {
        var seedRequested = args.Contains("--seed");
        var isDevelopment = app.Environment.IsDevelopment();

        if (!seedRequested && !isDevelopment)
            return false;

        await app.UseSeedingAsync();

        return seedRequested;
    }
}