using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using Store.Infrastructure.Repositories;

namespace Store.Infrastructure
{
    /// <summary>Registers infrastructure components with the application's dependency container.</summary>
    public static class InfrastructureRegistration
    {
        /// <summary>Registers the database context, generic repository, and unit of work.</summary>
        public static IServiceCollection InfrastructureConfig(this IServiceCollection services, IConfiguration configuration) // extension method to register infrastructure services
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("StoreDb")));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Apply Unit Of Work

            return services;
        }
    }
}
