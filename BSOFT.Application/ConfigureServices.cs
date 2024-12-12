using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using FluentValidation;
using MediatR;
using BSOFT.Application.Common.Behaviours;
using BSOFT.Application.Units.Commands.CreateUnit;
using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.Role.Queries.GetRolesAutocomplete;


namespace BSOFT.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Use a specific AddAutoMapper overload
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            // Add MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                //Validation
                cfg.AddBehavior(typeof(IPipelineBehavior<,>),typeof(ValidationBehaviour<,>));
                cfg.RegisterServicesFromAssemblies(
                                typeof(CreateRoleEntitlementCommandHandler).Assembly,
                                typeof(GetRolesAutocompleteQueryHandler).Assembly
    );
                
            });

            return services;
        }
    }
}
