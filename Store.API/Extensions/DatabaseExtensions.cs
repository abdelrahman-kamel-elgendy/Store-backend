using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Store.API.Configuration;
using Store.Infrastructure.Data;

namespace Store.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("StoreDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Required configuration 'ConnectionStrings:StoreDb' is missing. Set ConnectionStrings__StoreDb in the repository-root .env file or the host environment.");

        services.AddOptions<DatabaseOptions>()
            .Configure(o => o.ConnectionString = connectionString)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SeedDatabaseOptions>()
            .Configure(o => o.Enabled = configuration.GetValue<bool>("SEED_DATABASE"))
            .Validate(o => !o.Enabled || environment.IsDevelopment(), "SEED_DATABASE=true is only allowed when ASPNETCORE_ENVIRONMENT=Development.")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) => options.UseSqlServer(sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));

        return services;
    }
}