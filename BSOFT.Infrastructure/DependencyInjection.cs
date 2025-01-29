using Microsoft.EntityFrameworkCore;
using BSOFT.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using System.Data;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using Core.Application.Common.Mappings;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.ICountry;
using BSOFT.Infrastructure.Repositories.Country;
using Core.Application.Common.Interfaces.IState;
using BSOFT.Infrastructure.Repositories.State;
using Core.Application.Common.Interfaces.ICity;
using BSOFT.Infrastructure.Repositories.City;
using BSOFT.Infrastructure.Repositories.Users;
using BSOFT.Infrastructure.Repositories.RoleEntitlements;
using BSOFT.Infrastructure.Repositories.Module;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using Core.Application.Common.Interfaces.IModule;
using BSOFT.Infrastructure.Repositories.Departments;
using Core.Application.Common.Interfaces.IDepartment;
using BSOFT.Infrastructure.Repositories.UserRoles;
using Core.Application.Common.Interfaces.IUserRole;
using BSOFT.Infrastructure.Repositories.Companies;
using Core.Application.Common.Interfaces.ICompany;
using BSOFT.Infrastructure.Repositories.Units;
using Core.Application.Common.Interfaces.IUnit;
using BSOFT.Infrastructure.Repositories.Entities;
using Core.Application.Common.Interfaces.IEntity;
using BSOFT.Infrastructure.Repositories.Divisions;
using Core.Application.Common.Interfaces.IDivision;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUserRoleAllocation;
using BSOFT.Infrastructure.Repositories.UserRoleAllocation.UserRoleAllocationQueryRepository;
using BSOFT.Infrastructure.Repositories.UserRoleAllocation.UserRoleAllocationCommandRepository;
using Core.Application.Common.Interfaces.AuditLog;
using Infrastructure.Data;
using BSOFT.Infrastructure.Logging;
using Serilog;
using BSOFT.Infrastructure.Resilience;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Application.Notification.Queries;
using Core.Application.Common.Interfaces.INotifications;
using BSOFT.Infrastructure.Repositories.Notifications;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using BSOFT.Infrastructure.Repositories.PasswordComplexityRule;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using BSOFT.Infrastructure.Repositories.AdminSecuritySettings;
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
            services.AddTransient<IDbConnection>(sp => new SqlConnection(connectionString));
    
    
             // MongoDB Context
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoConnectionString = configuration.GetConnectionString("MongoDbConnectionString");
            if (string.IsNullOrWhiteSpace(mongoConnectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is missing or empty.");
            }
            return new MongoClient(mongoConnectionString);
        });

        services.AddSingleton<IMongoDbContext>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDb:DatabaseName"];
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new InvalidOperationException("MongoDB database name is missing or empty.");
            }
            return new MongoDbContext(client, databaseName);
        });

      // Optional: Register IMongoDatabase if needed directly
        services.AddSingleton(sp =>
        {
            var mongoDbContext = (MongoDbContext)sp.GetRequiredService<IMongoDbContext>();
            return mongoDbContext.GetDatabase();
        });

            // Configure JWT settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));       

        // Register ILogger<T>
        services.AddLogging(builder =>
        {
            builder.AddSerilog();
        }); 
        // Register Polly Policies
        services.AddPollyPolicies(configuration);
            

            // Register repositories
             services.AddScoped<IUserQueryRepository, UserQueryRepository>();
            services.AddScoped<IUserCommandRepository, UserCommandRepository>();
            services.AddScoped<IUserRoleAllocationQueryRepository, UserRoleAllocationQueryRepository>();
            services.AddScoped<IUserRoleAllocationCommandRepository, UserRoleAllocationCommandRepository>();
            services.AddScoped<IRoleEntitlementCommandRepository, RoleEntitlementCommandRepository>();
            services.AddScoped<IRoleEntitlementQueryRepository, RoleEntitlementQueryRepository>();
            services.AddScoped<IModuleCommandRepository, ModuleCommandRepository>();
            services.AddScoped<IModuleQueryRepository, ModuleQueryRepository>();
            services.AddScoped<IDepartmentCommandRepository, DepartmentCommandRepository>();
            services.AddScoped<IDepartmentQueryRepository, DepartmentQueryRepository>();
            services.AddScoped<IUserRoleCommandRepository, UserRoleCommandRepository>();
            services.AddScoped<IUserRoleQueryRepository, UserRoleQueryRepository>();
            services.AddScoped<ICompanyCommandRepository, CompanyCommandRepository>();
            services.AddScoped<ICompanyQueryRepository, CompanyQueryRepository>();
            services.AddScoped<IUnitCommandRepository, UnitCommandRepository>();
            services.AddScoped<IUnitQueryRepository, UnitQueryRepository>();
            services.AddScoped<IEntityCommandRepository, EntityCommandRepository>();
            services.AddScoped<IEntityQueryRepository, EntityQueryRepository>();
            services.AddScoped<IDivisionCommandRepository, DivisionCommandRepository>();
            services.AddScoped<IDivisionQueryRepository, DivisionQueryRepository>();
            services.AddScoped<ICountryCommandRepository, CountryCommandRepository>();
            services.AddScoped<ICountryQueryRepository, CountryQueryRepository>();
            services.AddScoped<IStateCommandRepository, StateCommandRepository>();
            services.AddScoped<IStateQueryRepository, StateQueryRepository>();
            services.AddScoped<ICityCommandRepository, CityCommandRepository>();
            services.AddScoped<ICityQueryRepository, CityQueryRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();    
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
 			services.AddTransient<NotificationsQueryHandler>();  
            services.AddTransient<INotificationsQueryRepository, NotificationsQueryRepository>();                             
			services.AddScoped<IPasswordComplexityRuleQueryRepository,  PasswordComplexityRuleQueryRepository>();
            services.AddScoped<IPasswordComplexityRuleCommandRepository, PasswordComplexityRuleCommandRepository>();
            services.AddScoped<IAdminSecuritySettingsQueryRepository,  AdminSecuritySettingsQueryRepository>();
            services.AddScoped<IAdminSecuritySettingsCommandRepository, AdminSecuritySettingsCommandRepository>();            
            services.AddHttpContextAccessor();            
            

            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();            
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();            
            services.AddScoped<IChangePassword, PasswordChangeRepository>();

            // AutoMapper profiles
            services.AddAutoMapper(
                typeof(UserProfile),
                typeof(RoleEntitlementMappingProfile),
                typeof(ModuleProfile),
                typeof(ChangePasswordProfile),             
				typeof(PasswordComplexityRuleProfile),
                typeof(EntityProfile),
                typeof(UnitProfile),
 				typeof(AdminSecuritySettingsProfile),
				typeof(DepartmentProfile),
                typeof(UpdateUnitProfile),
                typeof(CreateUnitProfile),
                typeof(UpdateUnitProfile)
            );

            return services;
        }
    }
}
