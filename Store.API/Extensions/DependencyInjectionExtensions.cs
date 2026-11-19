using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Store.API.Services;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Core.Mapping;
using Store.Core.Options;
using Store.Infrastructure.Repositories;
using Store.Service.Services;

namespace Store.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(CategoryMapping).Assembly);
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IApiResponseBuilder, ApiResponseBuilder>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();

        // Image service is stateless; singleton is fine now that options are injected
        services.AddSingleton<IImageManagementService, ImageManagmentService>();

        // IFileProvider follows the configured RootPath (resolved lazily)
        services.AddSingleton<IFileProvider>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var options = sp.GetRequiredService<IOptions<ImageStorageOptions>>().Value;

            var root = Path.IsPathRooted(options.RootPath) ? options.RootPath : Path.Combine(env.ContentRootPath, options.RootPath);

            Directory.CreateDirectory(root);
            return new PhysicalFileProvider(root);
        });

        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddImageStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ImageStorageOptions>()
            .Bind(configuration.GetSection(ImageStorageOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}