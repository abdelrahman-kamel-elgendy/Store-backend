using Microsoft.Extensions.FileProviders;
using Store.API.Services;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Services;
using Store.Core.Mapping;
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
        // Add the environment-aware builder
        services.AddScoped<IApiResponseBuilder, ApiResponseBuilder>();

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IImageManagementService, ImageManagmentService>();
        services.AddSingleton<IApiResponseBuilder, ApiResponseBuilder>();
        services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
        services.AddMemoryCache();
        return services;
    }
}