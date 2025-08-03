using CQRS.Application.Contracts.Interface;
using CQRS.Persistence.DBContext;
using CQRS.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CQRS.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<CQRSPersistenceDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CQRS_CleanArchConnectionString"));
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBatchSerialRepository, BatchSerialRepository>();
            services.AddScoped<IMainSerialRepository, MainSerialRepository>();


            return services;
        }
    }
}
