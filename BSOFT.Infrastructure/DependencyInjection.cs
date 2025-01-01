using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Common.Mappings;
using Core.Application.Common.Interface;
using System.Data;
using Microsoft.Data.SqlClient;


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

                // Register IDbConnection for Dapper
                services.AddTransient<IDbConnection>(sp =>
                {
                    return new SqlConnection(connectionString);
                });

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
                services.AddScoped<IRoleEntitlementRepository, RoleEntitlementRepository>();
                services.AddScoped<IModuleRepository, ModuleRepository>();
                services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                services.AddScoped<IUserRoleRepository, UserRoleRepository>();
                services.AddScoped<ICompanyRepository, CompanyRepository>();
 				services.AddScoped<ICompanyAddressRepository, CompanyAddressRepository>();
                services.AddScoped<ICompanyContactRepository, CompanyContactRepository>();
                services.AddScoped<IUnitRepository, UnitRepository>();
                services.AddScoped<IEntityRepository,EntityRepository>();
 				services.AddScoped<IDivisionRepository, DivisionRepository>();
                services.AddScoped<IIPAddressService, IPAddressService>();
				services.AddTransient<IFileUploadService, FileUploadRepository>();
                services.AddScoped<ICountryRepository, CountryRepository>();


                services.AddAutoMapper(typeof(CreateUserProfile), typeof(UpdateUserProfile));
                services.AddAutoMapper(typeof(RoleEntitlementMappingProfile));
                services.AddAutoMapper(typeof(ModuleProfile));
                services.AddAutoMapper(typeof(CompanyProfile));
  				services.AddAutoMapper(typeof(EntityProfile));
                services.AddAutoMapper(typeof(UnitProfile));

                return services;
            }
    }
}