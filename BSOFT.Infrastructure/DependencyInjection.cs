using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using Core.Application.Common.Interface;
using Core.Application.Common.Mappings;
using Core.Domain.Entities;


namespace BSOFT.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
            }

            // Register ApplicationDbContext with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

                // Register IDbConnection for Dapper
                services.AddTransient<IDbConnection>(sp =>
                {
                    return new SqlConnection(connectionString);
                });

    // Register MongoDbContext
        services.AddTransient<MongoDbContext>();

    services.AddTransient<IMongoDatabase>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDbConnectionString"));
    return mongoClient.GetDatabase(configuration["MongoDb:DatabaseName"]);
});
            //services.AddSingleton<IMongoClient>(sp => new MongoClient(mongoConnectionString));
            //services.AddSingleton<MongoDbContext>();

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleEntitlementRepository, RoleEntitlementRepository>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyAddressRepository, CompanyAddressRepository>();
            services.AddScoped<ICompanyContactRepository, CompanyContactRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IEntityRepository, EntityRepository>();
            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogMongoRepository>();
            services.AddScoped<IPasswordComplexityRepository, PasswordComplexityRepository>();
            services.AddScoped<IPasswordComplexityRepository, PasswordComplexityRepository>();

            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IJwtTokenHelper, JwtTokenHelper>();


            // AutoMapper profiles
            services.AddAutoMapper(
                typeof(CreateUserProfile),
                typeof(UpdateUserProfile),
                typeof(RoleEntitlementMappingProfile),
                typeof(ModuleProfile),
                typeof(CompanyProfile),
                typeof(AuditLogMappingProfile),
                typeof(PasswordComplexityRuleProfile)
            );

            return services;
        }
    }
}
