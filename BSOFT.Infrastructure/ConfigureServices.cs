using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BSOFT.Domain.Interfaces;
using BSOFT.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
            }
                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

                services.AddTransient<IUserRepository, UserRepository>();
                services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
                services.AddTransient<IRoleEntitlementRepository, RoleEntitlementRepository>();
                // services.AddAutoMapper(typeof(CreateUserProfile));
                // services.AddAutoMapper(typeof(UpdateUserProfile));
                // services.AddAutoMapper(typeof(RoleEntitlementProfile));
                services.AddAutoMapper(typeof(CreateUserProfile), typeof(UpdateUserProfile), typeof(RoleEntitlementProfile));


                services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                
                services.AddScoped<ICompanyRepository, CompanyRepository>();

            // Add FluentValidation
                services.AddControllers()
                    .AddFluentValidation(fv =>
                    {
                        fv.RegisterValidatorsFromAssemblyContaining<CreateRoleEntitlementCommandValidator>();
                    }); 

                services.AddTransient<IValidator<CreateRoleEntitlementVm>, CreateRoleEntitlementCommandValidator>();


                return services;
            }
    }
}