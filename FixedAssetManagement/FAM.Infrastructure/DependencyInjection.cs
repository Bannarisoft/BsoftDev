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
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral;
using Core.Application.Common.Interfaces.IUOM;
using FAM.Infrastructure.Repositories.UOMs;
using Core.Application.Common.Mappings.AssetMaster;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using FAM.Infrastructure.Repositories.AssetMaster.AssetPurchase;
using Core.Application.Common.Mappings.AssetPurchase;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using FAM.Infrastructure.Repositories.SpecificationMaster;
using FAM.Infrastructure.Repositories.AssetMaster.AssetSpecification;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using FAM.Infrastructure.Repositories.AssetMaster.AssetLocation;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost;
using FAM.Infrastructure.Repositories.AssetMaster.AssetAdditionalCost;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using FAM.Infrastructure.Repositories.AssetMaster.AssetWarranty;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;

using FAM.Infrastructure.Repositories.AssetMaster.AssetInsurance;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using FAM.Infrastructure.Repositories.AssetMaster.AssetAmc;


using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetDisposal;
using FAM.Infrastructure.Repositories.AssetMaster.AssetDisposal;

using Core.Application.Common.Interfaces.IDepreciationDetail;
using FAM.Infrastructure.Repositories.DepreciationDetail;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using FAM.Infrastructure.Repositories.AssetMaster.AssetTransferIssue;
using FAM.Infrastructure.Repositories.AssetTransferIssueApproval;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using FAM.Infrastructure.Repositories.AssetTransferReceipt;
using FAM.Infrastructure.Repositories.AssetMaster.AssetTransfer;
using Core.Application.Common.Interfaces.IExcelImport;
using FAM.Infrastructure.Repositories.ExcelImport;
using FAM.Infrastructure.Helpers;
using Core.Application.Common.Interfaces.IReports;
using FAM.Infrastructure.Repositories.Reports;
using FAM.Infrastructure.Repositories.AssetSubGroup;
using Core.Application.Common.Interfaces.IAssetSubGroup;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using FAM.Infrastructure.Repositories.WDVDepreciation;

namespace FAM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {

            // var connectionString = ConnectionStringHelper.GetDefaultConnectionString(configuration);
            // var HangfireConnectionString = ConnectionStringHelper.GetHangfireConnectionString(configuration);

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                                .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
                                                .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
                                                .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");

            // var HangfireConnectionString = configuration.GetConnectionString("HangfireConnection")
            //                                     .Replace("{SERVER}", Environment.GetEnvironmentVariable("DATABASE_SERVER") ?? "")
            //                                     .Replace("{USER_ID}", Environment.GetEnvironmentVariable("DATABASE_USERID") ?? "")
            //                                     .Replace("{ENC_PASSWORD}", Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "");


            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found or is empty.");
            }
            // if (string.IsNullOrWhiteSpace(HangfireConnectionString))
            // {
            //     throw new InvalidOperationException("Connection string 'HangfireConnectionString' not found or is empty.");
            // }

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
            // services.AddHangfire(config =>
            // {
            //     config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            //           .UseSimpleAssemblyNameTypeSerializer()
            //           .UseDefaultTypeSerializer()
            //           .UseSqlServerStorage(HangfireConnectionString, new SqlServerStorageOptions
            //           {
            //               CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //               QueuePollInterval = TimeSpan.Zero,
            //               UseRecommendedIsolationLevel = true,
            //               UsePageLocksOnDequeue = true,
            //               DisableGlobalLocks = true
            //           });
            // });
            // // Add the Hangfire server
            // services.AddHangfireServer();

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
            services.AddScoped<IAssetMasterGeneralCommandRepository, AssetMasterGeneralCommandRepository>();
            services.AddScoped<IAssetMasterGeneralQueryRepository, AssetMasterGeneralQueryRepository>();
            services.AddScoped<IUOMCommandRepository, UOMCommandRepository>();
            services.AddScoped<IUOMQueryRepository, UOMQueryRepository>();
            services.AddScoped<IAssetPurchaseQueryRepository, AssetPurchaseQueryRepository>();
            services.AddScoped<IAssetPurchaseCommandRepository, AssetPurchaseCommandRepository>();
            services.AddScoped<ISpecificationMasterCommandRepository, SpecificationMasterCommandRepository>();
            services.AddScoped<ISpecificationMasterQueryRepository, SpecificationMasterQueryRepository>();
            services.AddScoped<IAssetSpecificationCommandRepository, AssetSpecificationCommandRepository>();
            services.AddScoped<IAssetLocationQueryRepository, AssetLocationQueryRepository>();
            services.AddScoped<IAssetLocationCommandRepository, AssetLocationCommandRepository>();
            services.AddScoped<IAssetSpecificationQueryRepository, AssetSpecificationQueryRepository>();
            services.AddScoped<IAssetAdditionalCostQueryRepository, AssetAdditionalCostQueryRepository>();
            services.AddScoped<IAssetAdditionalCostCommandRepository, AssetAdditionalCostCommandRepository>();
            services.AddScoped<IAssetWarrantyQueryRepository, AssetWarrantyQueryRepository>();
            services.AddScoped<IAssetWarrantyCommandRepository, AssetWarrantyCommandRepository>();
            services.AddScoped<IAssetInsuranceCommandRepository, AssetInsuranceCommandRepository>();
            services.AddScoped<IAssetInsuranceQueryRepository, AssetInsuranceQueryRepository>();
            services.AddScoped<IAssetAmcQueryRepository, AssetAmcQueryRepository>();
            services.AddScoped<IAssetAmcCommandRepository, AssetAmcCommandRepository>();
            services.AddScoped<IAssetTransferQueryRepository, AssetTransferQueryRepository>();
            services.AddScoped<IAssetTransferCommandRepository, AssetTransferCommandRepository>();
            services.AddScoped<IAssetDisposalQueryRepository, AssetDisposalQueryRepository>();
            services.AddScoped<IAssetDisposalCommandRepository, AssetDisposalCommandRepository>();
            services.AddScoped<IDepreciationDetailCommandRepository, DepreciationDetailCommandRepository>();
            services.AddScoped<IDepreciationDetailQueryRepository, DepreciationDetailQueryRepository>();
            services.AddScoped<IAssetTransferIssueApprovalQueryRepository, AssetTransferIssueQueryRepository>();
            services.AddScoped<IAssetTransferIssueApprovalCommandRepository, AssetTransferIssueCommandRepository>();
            services.AddScoped<IAssetTransferReceiptQueryRepository, AssetTransferReceiptQueryRepository>();
            services.AddScoped<IAssetTransferReceiptCommandRepository, AssetTransferReceiptCommandRepository>();
            services.AddScoped<IExcelImportCommandRepository, ExcelImportCommandRepository>();
            services.AddScoped<IExcelImportQueryRepository, ExcelImportCommandQueryRepository>();
            services.AddScoped<IReportRepository, ReportsRepository>();
            services.AddScoped<IAssetSubGroupCommandRepository, AssetSubGroupCommandRepository>();
            services.AddScoped<IAssetSubGroupQueryRepository, AssetSubGroupQueryRepository>();
            services.AddScoped<IWdvDepreciationQueryRepository, WdvDepreciationQueryRepository>();            
            services.AddScoped<IWdvDepreciationCommandRepository, WdvDepreciationCommandRepository>(); 


            // Miscellaneous services
            services.AddScoped<IIPAddressService, IPAddressService>();
            services.AddTransient<IFileUploadService, FileUploadRepository>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>();
            services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
            services.AddTransient<ILocationLookupService, LocationLookupService>();

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
                typeof(UOMProfile),
                typeof(AssetMasterGeneralProfile),
                typeof(SpecificationMasterProfile),
                typeof(AssetPurchaseProfile),
                 typeof(AssetSpecificationProfile),
                typeof(AssetWarrantyProfile),
                typeof(AssetLocationProfile),
                typeof(AssetAdditionalCostProfile),
                typeof(AssetInsuranceProfile),
                typeof(AssetTransferProfile),
                typeof(AssetAmcProfile),
                typeof(AssetDisposalProfile),
                 typeof(DepreciationDetailProfile),
                typeof(AssetIssueTransferApproval),
                typeof(AssetTransferReceiptProfile),
                typeof(AssetAuditProfile),
                typeof(AssetSubGroupProfile),
                typeof(WDVDepreciationDetailProfile)
            );
            return services;
        }

    }
}
