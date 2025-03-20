using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Application.Common.Interfaces;
using UserManagement.Infrastructure.Repositories;
using System.Data;
using Microsoft.Data.SqlClient;
using MongoDB.Driver;
using Core.Application.Common.Mappings;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.ICountry;
using UserManagement.Infrastructure.Repositories.Country;
using Core.Application.Common.Interfaces.IState;
using UserManagement.Infrastructure.Repositories.State;
using Core.Application.Common.Interfaces.ICity;
using UserManagement.Infrastructure.Repositories.City;
using UserManagement.Infrastructure.Repositories.Users;
using UserManagement.Infrastructure.Repositories.RoleEntitlements;
using UserManagement.Infrastructure.Repositories.Module;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using Core.Application.Common.Interfaces.IModule;
using UserManagement.Infrastructure.Repositories.Departments;
using Core.Application.Common.Interfaces.IDepartment;
using UserManagement.Infrastructure.Repositories.UserRoles;
using Core.Application.Common.Interfaces.IUserRole;
using UserManagement.Infrastructure.Repositories.Companies;
using Core.Application.Common.Interfaces.ICompany;
using UserManagement.Infrastructure.Repositories.Units;
using Core.Application.Common.Interfaces.IUnit;
using UserManagement.Infrastructure.Repositories.Entities;
using Core.Application.Common.Interfaces.IEntity;
using UserManagement.Infrastructure.Repositories.Divisions;
using Core.Application.Common.Interfaces.IDivision;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUserRoleAllocation;
using UserManagement.Infrastructure.Repositories.UserRoleAllocation.UserRoleAllocationQueryRepository;
using UserManagement.Infrastructure.Repositories.UserRoleAllocation.UserRoleAllocationCommandRepository;
using Core.Application.Common.Interfaces.AuditLog;
using Infrastructure.Data;
using Serilog;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Application.Notification.Queries;
using Core.Application.Common.Interfaces.INotifications;
using UserManagement.Infrastructure.Repositories.Notifications;
using Core.Application.Common.Interfaces.IPasswordComplexityRule;
using UserManagement.Infrastructure.Repositories.PasswordComplexityRule;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using UserManagement.Infrastructure.Repositories.AdminSecuritySettings;
using Hangfire;
using Hangfire.SqlServer;
using UserManagement.Infrastructure.Services;
using Core.Domain.Common;
using Core.Application.Common.Interfaces.ICompanySettings;
using UserManagement.Infrastructure.Repositories.CompanySettings;
using Core.Application.Common.Interfaces.ICurrency;
using UserManagement.Infrastructure.Repositories.Currency;
using Core.Application.Common.Interfaces.ITimeZones;
using UserManagement.Infrastructure.Repositories.TimeZones;
using UserManagement.Infrastructure.PollyResilience;
using Core.Application.Common.Interfaces.ILanguage;
using UserManagement.Infrastructure.Repositories.Language;
using Core.Application.Common.Interfaces.IFinancialYear;
using UserManagement.Infrastructure.Repositories.FinancialYear;
using Core.Application.Common.Interfaces.IMenu;
using UserManagement.Infrastructure.Repositories.Menu;
using Core.Application.Common.Interfaces.IProfile;
using AutoMapper;
using UserManagement.Infrastructure.Repositories.Profile;
using Core.Application.Common.Interfaces.IUserGroup;
using UserManagement.Infrastructure.Repositories.UserGroup;
using System.Text;
using System.Security.Cryptography;
namespace UserManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
            {
                var encryptionKey = configuration["Encryption:AESKey"];

            if (string.IsNullOrWhiteSpace(encryptionKey))
                throw new InvalidOperationException("Encryption key is missing in appsettings.json.");

            // Get encrypted values from environment variables or appsettings.json
            var encryptedPassword = FetchOrSetEnvironmentVariable("DATABASE_PASSWORD", configuration["Encryption:DefaultEncryptedPassword"]);
            var encryptedServer = FetchOrSetEnvironmentVariable("DATABASE_SERVER", configuration["Encryption:DefaultEncryptedServer"]);
            var encryptedUserId = FetchOrSetEnvironmentVariable("DATABASE_USERID", configuration["Encryption:DefaultEncryptedUserId"]);

            // Check if any of the encrypted values are null or empty
            if (string.IsNullOrWhiteSpace(encryptedPassword))
                throw new InvalidOperationException("Encrypted password is missing.");

            if (string.IsNullOrWhiteSpace(encryptedServer))
                throw new InvalidOperationException("Encrypted server is missing.");

            if (string.IsNullOrWhiteSpace(encryptedUserId))
                throw new InvalidOperationException("Encrypted userId is missing.");

            // Decrypt all the values
            var decryptedPassword = Decrypt(encryptedPassword, encryptionKey);
            var decryptedServer = Decrypt(encryptedServer, encryptionKey);
            var decryptedUserId = Decrypt(encryptedUserId, encryptionKey);

            // Replace placeholders in connection strings
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                               .Replace("{SERVER}", decryptedServer)
                                               .Replace("{USER_ID}", decryptedUserId)
                                               .Replace("{ENC_PASSWORD}", decryptedPassword);

            var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                                                        .Replace("{SERVER}", decryptedServer)
                                                        .Replace("{USER_ID}", decryptedUserId)
                                                        .Replace("{ENC_PASSWORD}", decryptedPassword);
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
                } 
                if (string.IsNullOrWhiteSpace(HangfireConnectionString))
                {
                    throw new InvalidOperationException("Connection string 'HangfireConnectionString' not found or is empty.");
                }

            // Register ApplicationDbContext with SQL Server
           /*  services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)); */
                services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5, // Number of retry attempts
                    maxRetryDelay: TimeSpan.FromSeconds(30), // Delay between retries
                    errorNumbersToAdd: null); // Add specific SQL error numbers to retry on (optional)
            }));

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

          // Register Hangfire services
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseDefaultTypeSerializer()
                      .UseSqlServerStorage(HangfireConnectionString, new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.Zero,
                          UseRecommendedIsolationLevel = true,
                          UsePageLocksOnDequeue = true,
                          DisableGlobalLocks = true
                      });
            });

            // Add the Hangfire server
            services.AddHangfireServer();

        // Register ILogger<T>
        services.AddLogging(builder =>
        {
            builder.AddSerilog();


        }); 
        // services.AddDistributedMemoryCache();
        // services.AddSession(options =>
        // {
        //     options.IdleTimeout = TimeSpan.FromHours(1);
        //     options.Cookie.HttpOnly = true;
        //     options.Cookie.IsEssential = true;
        // });
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
            services.AddScoped<IFinancialYearQueryRepository,  FinancialYearQueryRepository>();
            services.AddScoped<IFinancialYearCommandRepository , FinancialYearCommandRepository>();
            services.AddHttpContextAccessor();            
            services.AddScoped<ICompanyQuerySettings, CompanySettingsQueryRepository>();   
            services.AddScoped<ICurrencyQueryRepository, CurrencyQueryRepository>();
            services.AddScoped<ICurrencyCommandRepository, CurrencyCommandRepository>();
            services.AddScoped<ITimeZonesQueryRepository, TimeZonesQueryRepository>();
            services.AddScoped<ICompanyCommandSettings, CompanySettingsCommandRepository>();
            services.AddScoped<ICompanyQuerySettings, CompanySettingsQueryRepository>();
            services.AddScoped<ILanguageCommand, LanguageCommandRepository>();
            services.AddScoped<ILanguageQuery, LanguageQueryRepository>(); 
            services.AddScoped<IMenuQuery, MenuQueryRepository>();
            services.AddScoped<IProfileQuery, ProfileQueryRepository>();
            services.AddScoped<IProfileCommand, ProfileCommandRepository>();
            services.AddScoped<IUserGroupQueryRepository, UserGroupQueryRepository>();
            services.AddScoped<IUserGroupCommandRepository, UserGroupCommandRepository>();
            
            
            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();            
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();            
            services.AddSingleton<ITimeZoneService, TimeZoneService>(); 
            services.AddScoped<IChangePassword, PasswordChangeRepository>();            
            

            /*services.Configure<EmailJobSettings>(configuration.GetSection("EmailJobSettings"));
            services.Configure<EmailJobSettings>(configuration.GetSection("EmailSettings"));            
             services.AddHostedService<EmailJobService>();     */          
            services.AddHttpClient(); 
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
            services.AddSingleton<IEmailService,EmailService>();            
            services.AddSingleton<ISmsService, SmsService>();

            // AutoMapper profiles
            services.AddAutoMapper(
                typeof(UserProfile),
                typeof(RoleEntitlementMappingProfile),
                typeof(ModuleProfile),
                typeof(ChangePasswordProfile),             
				typeof(PasswordComplexityRuleProfile),
                typeof(EntityProfile),
 				typeof(AdminSecuritySettingsProfile),
				typeof(DepartmentProfile),
                typeof(FinancialYearProfile),
                typeof(CurrencyProfile),
                typeof(UnitsProfile),
				typeof(CompanySettingsProfile)
            );

            return services;
        }
          // ✅ AES Decryption Method
        private static string Decrypt(string encryptedText, string key)
        {
            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] iv = new byte[16]; // AES IV

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                return "Decryption Failed!";
            }
        }
          // Method to fetch or set environment variables
        private static string FetchOrSetEnvironmentVariable(string variableName, string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            if (string.IsNullOrWhiteSpace(value))
            {                
                
                if (string.IsNullOrWhiteSpace(defaultValue))
                    throw new InvalidOperationException($"{variableName} is missing and no default value is provided.");

                // Set the environment variable
                Environment.SetEnvironmentVariable(variableName, defaultValue, EnvironmentVariableTarget.User);                
                
                // Return the default value for further processing
                value = defaultValue;
            }
            return value;
        }
    }
}
