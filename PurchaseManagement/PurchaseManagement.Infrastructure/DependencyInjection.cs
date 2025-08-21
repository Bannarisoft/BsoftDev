using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.AuditLog;
using Core.Application.Common.Interfaces.ILogService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IMiscTypeMaster;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Application.Common.Mappings;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using PurchaseManagement.Infrastructure.Data;
using PurchaseManagement.Infrastructure.Repositories;
using PurchaseManagement.Infrastructure.Repositories.LogServices;
using PurchaseManagement.Infrastructure.Repositories.MiscMaster;
using PurchaseManagement.Infrastructure.Repositories.MiscTypeMaster;
using PurchaseManagement.Infrastructure.Repositories.PurchaseIndents;
using PurchaseManagement.Infrastructure.Repositories.PartyMaster;
using PurchaseManagement.Infrastructure.Services;
using Serilog;

namespace PurchaseManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                                .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                                .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                                .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");


            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
            }

            // Register ApplicationDbContext with SQL Server

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


            // Register ILogger<T>
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            // Register IDateTime
            services.AddHttpContextAccessor();
            services.AddTransient<AuthTokenHandler>();

            // Register repositories
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IMiscTypeMasterQueryRepository, MiscTypeMasterQueryRepository>();            
            services.AddScoped<IMiscTypeMasterCommandRepository, MiscTypeMasterCommandRepository>();
            services.AddScoped<IMiscMasterQueryRepository, MiscMasterQueryRepository>();
            services.AddScoped<IMiscMasterCommandRepository, MiscMasterCommandRepository>();
            services.AddScoped<IPurchaseIndentCommand, PurchaseIndentCommandRepository>();
            services.AddScoped<ILogServiceCommand, LogServiceCommandRepository>();
            services.AddScoped<IPurchaseIndentQuery, PurchaseIndentQueryRepository>();
            services.AddScoped<IPartyMasterQueryRepository, PartyMasterQueryRepository>();
            services.AddScoped<IPurchaseIndentGrpcQuery, PurchaseIndentQueryRepository>();
            


            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();

            // AutoMapper profiles
            services.AddAutoMapper(
                typeof(MiscTypeMasterProfile),
                typeof(MiscMasterProfile)
                
            );
            return services;
        }

    }
}
