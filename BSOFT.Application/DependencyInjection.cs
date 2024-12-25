using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using BSOFT.Application.Common.Behaviors;
using BSOFT.Application.Common.Mappings;


namespace BSOFT.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Use a specific AddAutoMapper overload
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(typeof(UnitProfile).Assembly);
            services.AddAutoMapper(typeof(CreateUnitProfile).Assembly);
            services.AddAutoMapper(typeof(UpdateUnitProfile).Assembly);
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                //Validation
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });
            return services;
        }
    }
}
