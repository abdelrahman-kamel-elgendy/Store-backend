using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Interfaces;
using Store.Infrastructure.Data;
using Store.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Infrastructure
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection InfrastructureConfig(this IServiceCollection services, IConfiguration configuration) // extension method to register infrastructure services
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("StoreDb")));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Apply Unit Of Work

            return services;
        }
    }
}
