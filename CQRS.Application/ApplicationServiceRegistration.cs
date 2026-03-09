using CQRS.Application.Features;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CQRS.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMappe
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // MediatR + Pipeline Behaviors
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // Register Validation Pipeline
                cfg.AddOpenBehavior(typeof(ValidationHandlingMiddleware<,>));
            });
            // Register *all* FluentValidators in this assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
