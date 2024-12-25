using BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using BSOFT.Application.RoleEntitlements.Queries.GetRoles;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSOFT.Domain.Common.Interface;
using System.Data;
using Microsoft.Data.SqlClient;
using BSOFT.Application.Country.Queries.GetCountries;
using BSOFT.Application.Common.Mappings;


namespace BSOFT.Infrastructure
{
    public static class DependencyInjection
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

                services.AddTransient<IDbConnection>(_ => new SqlConnection(connectionString));                                

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
                services.AddTransient<IRoleEntitlementRepository, RoleEntitlementRepository>();
                services.AddAutoMapper(typeof(CreateUserProfile), typeof(UpdateUserProfile), typeof(RoleEntitlementProfile));


                services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                
                services.AddScoped<ICompanyRepository, CompanyRepository>();
				services.AddScoped<IUnitRepository, UnitRepository>();
                services.AddScoped<IEntityRepository,EntityRepository>();
 				services.AddScoped<IDivisionRepository, DivisionRepository>();
                services.AddScoped<IIPAddressService, IPAddressService>();
                services.AddScoped<ICountryRepository, CountryRepository>();
                services.AddAutoMapper(typeof(CountryProfile).Assembly);
                
                return services;
            }
    }
}