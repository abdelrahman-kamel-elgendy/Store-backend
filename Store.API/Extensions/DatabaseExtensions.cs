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

        services.AddOptions<DatabaseOpts>()
            .Configure(o => o.ConnectionString = connectionString)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) => options.UseSqlServer(sp.GetRequiredService<IOptions<DatabaseOpts>>().Value.ConnectionString));

        return services;
    }
}