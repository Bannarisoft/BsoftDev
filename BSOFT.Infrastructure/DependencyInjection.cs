using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
                services.AddScoped<IRoleEntitlementRepository, RoleEntitlementRepository>();
                services.AddScoped<IModuleRepository, ModuleRepository>();
                services.AddAutoMapper(typeof(CreateUserProfile), typeof(UpdateUserProfile));
                services.AddAutoMapper(typeof(RoleEntitlementMappingProfile));
                services.AddAutoMapper(typeof(ModuleProfile));



                services.AddScoped<IDepartmentRepository, DepartmentRepository>();
                services.AddScoped<IUserRoleRepository, UserRoleRepository>();
                
                services.AddScoped<ICompanyRepository, CompanyRepository>();
 				services.AddScoped<ICompanyAddressRepository, CompanyAddressRepository>();
                services.AddScoped<ICompanyContactRepository, CompanyContactRepository>();
                 services.AddAutoMapper(typeof(CompanyProfile));
				services.AddScoped<IUnitRepository, UnitRepository>();
                services.AddScoped<IEntityRepository,EntityRepository>();
 				services.AddScoped<IDivisionRepository, DivisionRepository>();
                services.AddScoped<IIPAddressService, IPAddressService>();
				services.AddTransient<IFileUploadService, FileUploadRepository>();

                return services;
            }
    }
}