using Microsoft.Extensions.FileProviders;
using Store.API.Services;
using Store.Core.Interfaces.Repositories;
using Store.Core.Interfaces.Servics;
using Store.Core.Interfaces.Services;
using Store.Infrastructure.Data.Seeding;
using Store.Infrastructure.Repositories;

namespace Store.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<DbSeeder>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IImageManagementService, ImageManagmentService>();
        services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
        return services;
    }
}