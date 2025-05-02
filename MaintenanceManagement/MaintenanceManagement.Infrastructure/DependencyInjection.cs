using System.Data;
using Core.Application.Common.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Serilog;
using MaintenanceManagement.Infrastructure.Data;
using MaintenanceManagement.Infrastructure.Services;
using Core.Application.Common.Interfaces.IMachineGroup;
using System.Reflection.PortableExecutable;
using MaintenanceManagement.Infrastructure.Repositories.MachineGroup;
using Core.Application.Common.Mappings;
using Core.Application.Common.Interfaces.IMiscTypeMaster;
using MaintenanceManagement.Infrastructure.Repositories.MiscTypeMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using MaintenanceManagement.Infrastructure.Repositories.MiscMaster;
using Core.Application.Common.Interfaces.IShiftMaster;
using Core.Domain.Entities;
using MaintenanceManagement.Infrastructure.Repositories.ShiftMaster;
using Core.Application.Common.Interfaces.IShiftMasterDetail;
using MaintenanceManagement.Infrastructure.Repositories.ShiftMasterDetailRepo;
using Core.Application.Common.Interfaces.ICostCenter;
using MaintenanceManagement.Infrastructure.Repositories.CostCenter;

using Core.Application.Common.Interfaces.IWorkCenter;
using MaintenanceManagement.Infrastructure.Repositories.WorkCenter;
using Core.Application.Common.Interfaces.IMaintenanceType;
using MaintenanceManagement.Infrastructure.Repositories.MaintenanceType;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using MaintenanceManagement.Infrastructure.Repositories.MaintenanceCategory;
using Core.Application.Common.Interfaces.IActivityMaster;
using MaintenanceManagement.Infrastructure.Repositories.ActivityMaster;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMachineGroupUser;
using MaintenanceManagement.Infrastructure.Repositories.MachineMaster;
using MaintenanceManagement.Infrastructure.Repositories.MachineGroupUser;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using MaintenanceManagement.Infrastructure.Repositories.ActivityCheckListMaster;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using MaintenanceManagement.Infrastructure.Repositories.MaintenanceRequest;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MaintenanceManagement.Infrastructure.Repositories.PreventiveSchedulers;
using MaintenanceManagement.Infrastructure.Repositories.WorkOrder;

using Core.Application.Common.Interfaces.IItem;
using MaintenanceManagement.Infrastructure.Repositories.Item;
using MaintenanceManagement.Infrastructure.Data.Configurations;
using Core.Application.Common.Interfaces.IStcokLedger;
using MaintenanceManagement.Infrastructure.Repositories.StockLedger;
using Core.Application.Common.Interfaces.IMainStoreStock;
using MaintenanceManagement.Infrastructure.Repositories;
using Core.Application.MainStoreStock.Queries.GetMainStoreStock;
using MaintenanceManagement.Infrastructure.Repositories.MainStoreStock;
using Core.Application.Common.Interfaces.IMRS;
using MaintenanceManagement.Infrastructure.Repositories.MRS;

namespace MaintenanceManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {


              var connectionString = configuration.GetConnectionString("DefaultConnection")
                                                .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                                .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                                .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");
            var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
                                                .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                                .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                                .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");    
            


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
            services.AddTransient<AuthTokenHandler>();

            // Register repositories
            services.AddScoped<ICostCenterQueryRepository, CostCenterQueryRepository>();
            services.AddScoped<ICostCenterCommandRepository, CostCenterCommandRepository>();
            services.AddScoped<IWorkCenterQueryRepository, WorkCenterQueryRepository>();
            services.AddScoped<IWorkCenterCommandRepository, WorkCenterCommandRepository>();

            services.AddScoped<IMachineGroupCommandRepository, MachineGroupCommandRepository>();
            services.AddScoped<IMachineGroupQueryRepository, MachineGroupQueryRepository>();
            services.AddScoped<IMiscTypeMasterCommandRepository, MiscTypeMasterCommandRepository>();
            services.AddScoped<IMiscTypeMasterQueryRepository, MiscTypeMasterQueryRepository>();
            services.AddScoped<IMiscMasterCommandRepository, MiscMasterCommandRepository>();
            services.AddScoped<IMiscMasterQueryRepository, MiscMasterQueryRepository>();
            services.AddScoped<IShiftMasterQuery, ShiftMasterQueryRepository>();
            services.AddScoped<IShiftMasterCommand, ShiftMasterCommandRepository>();
            services.AddScoped<IShiftMasterDetailQuery, ShiftMasterDetailQueryRepository>();
            services.AddScoped<IShiftMasterDetailCommand, ShiftMasterDetailCommandRepository>();
            services.AddScoped<IMaintenanceTypeCommandRepository, MaintenanceTypeCommandRepository>();
            services.AddScoped<IMaintenanceTypeQueryRepository, MaintenanceTypeQueryRepository>();
            services.AddScoped<IMaintenanceCategoryCommandRepository, MaintenanceCategoryCommandRepository>();
            services.AddScoped<IMaintenanceCategoryQueryRepository, MaintenanceCategoryQueryRepository>();

            services.AddScoped<IActivityMasterQueryRepository, ActivityMasterQueryRepository>();


            services.AddScoped<IActivityMasterCommandRepository, ActivityMasterCommandRepository>();

            services.AddScoped<IMachineGroupUserQueryRepository, MachineGroupUserQueryRepository>();
            services.AddScoped<IMachineGroupUserCommandRepository, MachineGroupUserCommandRepository>();

            services.AddScoped<IMachineMasterCommandRepository, MachineMasterCommandRepository>();
            services.AddScoped<IMachineMasterQueryRepository, MachineMasterQueryRepository>();

             services.AddScoped<IWorkOrderCommandRepository, WorkOrderCommandRepository>();
             services.AddScoped<IWorkOrderQueryRepository, WorkOrderQueryRepository>();

            services.AddScoped<IActivityCheckListMasterQueryRepository, ActivityCheckListMasterQueryRepository>();
            services.AddScoped<IActivityCheckListMasterCommandRepository, ActivityCheckListMasterCommandRepository>();
            services.AddScoped<IMaintenanceRequestQueryRepository, MaintenanceRequestQueryRepository>();
            services.AddScoped<IMaintenanceRequestCommandRepository, MaintenanceRequestCommandRepository>();
            
            services.AddScoped<IPreventiveSchedulerCommand, PreventiveSchedulerCommandRepository>();
            services.AddScoped<IPreventiveSchedulerQuery, PreventiveSchedulerQueryRepository>();
            services.AddScoped<IItemQueryRepository, ItemQueryRepository>();
            services.AddScoped<IStockLedgerQueryRepository, StockLedgerQueryRepository>();
            services.AddScoped<IMainStoreStockQueryRepository, MainStoreStockQueryRepository>();
            services.AddScoped<IMRSQueryRepository, MRSQueryRepository>();
            services.AddScoped<IMRSCommandRepository, MRSCommandRepository>();

            // Miscellaneous services

            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            
            services.AddSingleton<ITimeZoneService, TimeZoneService>();

            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();                     
            services.AddScoped<ILogQueryService, LogQueryService>();   
            
            // AutoMapper profiles
            services.AddAutoMapper(
            typeof(MachineGroupProfile),
            typeof(MiscTypeMasterProfile),
            typeof(MiscMasterProfile),
            typeof(CostCenterProfile),

            typeof(WorkCenterProfile),
            typeof(MaintenanceTypeProfile),
            typeof(MaintenanceCategoryProfile),
            typeof(ShiftMasterProfile),
            typeof(ShiftMasterDetailProfile),

            typeof(ActivityMasterProfile),


            typeof(MachineGroupUserProfile),
            typeof(WorkOrderProfile),            
            typeof(ActivityCheckListMasterProfile),
            typeof(ItemProfile)



         );
            return services;
        }
    }
}