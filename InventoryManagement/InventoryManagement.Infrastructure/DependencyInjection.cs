using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.AuditLog;
using Core.Application.Common.Interfaces.Budget;
using Core.Application.Common.Interfaces.IHSNMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IMiscTypeMaster;
using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Common.Interfaces.Item.ItemDetail;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Application.Common.Interfaces.Item.Templates;
using Core.Application.Common.Interfaces.IUOM;
using Core.Application.Common.Interfaces.IUOMConversion;
using Core.Application.Common.Mappings;
using Core.Application.Common.Mappings.Item.ItemDetail;
using Infrastructure.Data;
using Infrastructure.Persistence.Repositories;
using InventoryManagement.Infrastructure.Data;
using InventoryManagement.Infrastructure.Repositories;
using InventoryManagement.Infrastructure.Repositories.HSNMaster;
using InventoryManagement.Infrastructure.Repositories.Item.ItemCategory;
using InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands;
using InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Queries;
using InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Repositories.Item.ItemGroup;
using InventoryManagement.Infrastructure.Repositories.Item.Templates;
using InventoryManagement.Infrastructure.Repositories.MiscMaster;
using InventoryManagement.Infrastructure.Repositories.MiscTypeMaster;
using InventoryManagement.Infrastructure.Repositories.UOMConversion;
using InventoryManagement.Infrastructure.Repositories.UOMs;
using InventoryManagement.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;

namespace InventoryManagement.Infrastructure
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
            services.AddScoped<IItemGroupCommandRepository, ItemGroupCommandRepository>();
            services.AddScoped<IItemGroupQueryRepository, ItemGroupQueryRepository>();
            services.AddScoped<IItemCategoryQueryRepository, ItemCategoryQueryRepository>();
            services.AddScoped<IItemCategoryCommandRepository, ItemCategoryCommandRepository>();    
			services.AddScoped<IMiscTypeMasterQueryRepository, MiscTypeMasterQueryRepository>();            
            services.AddScoped<IMiscTypeMasterCommandRepository, MiscTypeMasterCommandRepository>();
            services.AddScoped<IMiscMasterQueryRepository, MiscMasterQueryRepository>();

            services.AddScoped<IMiscMasterCommandRepository, MiscMasterCommandRepository>(); 
            services.AddScoped<IHSNMasterQueryRepository , HSNMasterQueryRepository>();     
            services.AddScoped<IHSNMasterCommandRepository, HSNMasterCommandRepository>();
            services.AddScoped<IUOMQueryRepository , UOMQueryRepository>();     
            services.AddScoped<IUOMCommandRepository, UOMCommandRepository>();
            services.AddScoped<IUOMConversionQueryRepository , UOMConversionQueryRepository>();
            services.AddScoped<IUOMConversionCommandRepository, UOMConversionCommandRepository>();
            services.AddScoped<IBudgetCommandRepository, BudgetCommandRepository>(); 
            services.AddScoped<IBudgetQueryRepository, BudgetQueryRepository>(); 
            services.AddScoped<IBudgetLogQueryRepository, BudgetLogQueryRepository>(); 
            //Item master
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IItemCommandRepository, ItemCommandRepository>();            
            services.AddScoped<IItemPurchaseCommandRepository, ItemPurchaseCommandRepository>();
            services.AddScoped<IItemInventoryCommandRepository, ItemInventoryCommandRepository>();
            services.AddScoped<IItemQualityCommandRepository, ItemQualityCommandRepository>();            
            services.AddScoped<IItemUomCommandRepository, ItemUomCommandRepository>();
            services.AddScoped<IItemManufactureCommandRepository, ItemManufactureCommandRepository>();
            services.AddScoped<IItemSupplierCommandRepository, ItemSupplierCommandRepository>();
            services.AddScoped<IItemVariantValueCommandRepository, ItemVariantValueCommandRepository>();
            services.AddScoped<IItemVariantValueQueryRepository, ItemVariantValueQueryRepository>();
            services.AddScoped<IItemQueryRepository, ItemQueryRepository>();
            services.AddScoped<ITemplateRepository,TemplateRepository>();
            


            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();

            // AutoMapper profiles
          services.AddAutoMapper(
                 typeof(MiscTypeMasterProfile),

                typeof(MiscMasterProfile),               
                typeof(HSNMasterProfile),
                typeof(UOMProfile),
                typeof(UOMConversionProfile),
                typeof(ItemProfile)
                
                
            );
            return services;
        }

    }
}

