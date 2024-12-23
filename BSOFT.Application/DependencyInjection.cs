// using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using BSOFT.Application.Common.Behaviors;


namespace BSOFT.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Use a specific AddAutoMapper overload
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                //Validation
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                // cfg.RegisterServicesFromAssembly(typeof(CreateRoleEntitlementCommandHandler).Assembly);
            });
            return services;
        }
    }
}
