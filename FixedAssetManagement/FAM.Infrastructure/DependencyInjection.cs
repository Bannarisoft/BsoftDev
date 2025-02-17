using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.AuditLog;
using Core.Application.Common.Interfaces.IAssetCategories;
using Core.Application.Common.Interfaces.IAssetGroup;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Application.Common.Interfaces.ILocation;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.Common.Mappings;
using FAM.Infrastructure.Data;
using FAM.Infrastructure.Repositories;
using FAM.Infrastructure.Repositories.AssetCategories;
using FAM.Infrastructure.Repositories.AssetGroup;
using FAM.Infrastructure.Repositories.DepreciationGroup;
using FAM.Infrastructure.Repositories.Locations;
using FAM.Infrastructure.Repositories.SubLocation;
using FAM.Infrastructure.Repositories.MiscTypeMaster;
using Core.Application.Common.Interfaces.IMiscTypeMaster;
using FAM.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;
using Core.Application.Common.Interfaces.IAssetSubCategories;
using FAM.Infrastructure.Repositories.AssetSubCategories;
using Core.Application.Common.Interfaces.IMiscMaster;
using FAM.Infrastructure.Repositories.MiscMaster;
using FAM.Infrastructure.Repositories.Manufacture;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUOM;
using FAM.Infrastructure.Repositories.UOMs;

namespace FAM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {
              var connectionString = configuration.GetConnectionString("DefaultConnection");
                var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
                } 
                if (string.IsNullOrWhiteSpace(HangfireConnectionString))
                {
                    throw new InvalidOperationException("Connection string 'HangfireConnectionString' not found or is empty.");
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

            // Register IDateTime
            services.AddHttpContextAccessor();

            // Register repositories
            services.AddScoped<IAuditLogRepository, AuditLogRepository>(); 
            services.AddScoped<IAssetGroupCommandRepository, AssetGroupCommandRepository>();
            services.AddScoped<ILocationCommandRepository, LocationCommandRepository>();
            services.AddScoped<ILocationQueryRepository, LocationQueryRepository>();
            services.AddScoped<ISubLocationCommandRepository, SubLocationCommandRepository>();
            services.AddScoped<ISubLocationQueryRepository, SubLocationQueryRepository>();   
            services.AddScoped<IMiscTypeMasterQueryRepository, MiscTypeMasterQueryRepository>(); 
            services.AddScoped<IMiscTypeMasterCommandRepository, MiscTypeMasterCommandRepository>();
            services.AddScoped<IDepreciationGroupCommandRepository, DepreciationGroupCommandRepository>();   
            services.AddScoped<IDepreciationGroupQueryRepository, DepreciationGroupQueryRepository>(); 
            services.AddScoped<IAssetGroupQueryRepository, AssetGroupQueryRepository>();
            services.AddScoped<IAssetCategoriesQueryRepository, AssetCategoriesQueryRepository>();
            services.AddScoped<IAssetCategoriesCommandRepository, AssetCategoriesCommandRepository>();
 			services.AddScoped<IAssetSubCategoriesQueryRepository, AssetSubCategoriesQueryRepository>();
            services.AddScoped<IAssetSubCategoriesCommandRepository, AssetSubCategoriesCommandRepository>();			
            services.AddScoped<IMiscMasterQueryRepository, MiscMasterQueryRepository>();
            services.AddScoped<IMiscMasterCommandRepository, MiscMasterCommandRepository>();            
			services.AddScoped<IManufactureCommandRepository, ManufactureCommandRepository>();
            services.AddScoped<IManufactureQueryRepository, ManufactureQueryRepository>();
            services.AddScoped<IUOMCommandRepository, UOMCommandRepository>();
            services.AddScoped<IUOMQueryRepository, UOMQueryRepository>();

            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>(); 
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>(); 

            // AutoMapper profiles
            services.AddAutoMapper(
				typeof(AssetGroupProfile),
				typeof(LocationProfile),
                typeof(SubLocationProfile),
                typeof(MiscTypeMasterProfile),
				typeof(MiscMasterProfile),
                typeof(DepreciationGroupProfile),
                typeof(AssetCategoriesProfile),
				typeof(AssetSubCategoriesProfile),
				typeof(ManufactureProfile),
                typeof(UOMProfile)
            );

            return services;
        }
    }
}