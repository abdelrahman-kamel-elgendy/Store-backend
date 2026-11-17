using Microsoft.Extensions.Options;
using Store.API.Configuration;
using Store.API.Helper;
using Store.API.MiddleWare;
using Store.Infrastructure.Data.Seeding;

namespace Store.API.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedDatabaseIfEnabledAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        if (!app.Services.GetRequiredService<IOptions<SeedDatabaseOptions>>().Value.Enabled) return;

        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();
    }

    public static WebApplication UseApiErrorHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseStatusCodePages(async ctx =>
        {
            var http = ctx.HttpContext;
            await http.Response.WriteAsJsonAsync(
                ApiResponses.FailureForStatusCode(http.Response.StatusCode, http.TraceIdentifier));
        });
        return app;
    }
}