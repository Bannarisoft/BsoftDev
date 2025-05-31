using Contracts.Interfaces.External.IFixedAssetManagement;
using Contracts.Interfaces.External.IMaintenance;
using Contracts.Interfaces.External.IUser;
using GrpcServices.Background;
using GrpcServices.FixedAssetManagement;
using GrpcServices.HangfireDelete;
using GrpcServices.UserManagement;
using MaintenanceManagement.Infrastructure.GrpcClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace MaintenanceManagement.Infrastructure
{
    public static class HttpClientInjection
    {
        private static readonly HttpClientHandler GrpcHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var userManagementUrl = configuration["GrpcSettings:UserManagementUrl"];
            var backGroundServiceUrl = configuration["GrpcSettings:BackGroundUrl"];
            var fixedassetmanagementUrl = configuration["GrpcSettings:FixedAssetUrl"];

            // ✅ Register Department gRPC Client
            services.AddGrpcClient<DepartmentService.DepartmentServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            // .AddGrpcPolicies();
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    // ❗ Use only in development to ignore SSL validation
                    // In production, use a valid certificate
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IDepartmentGrpcClient, DepartmentGrpcClient>();

            // ✅ Register Session gRPC Client
            services.AddGrpcClient<SessionService.SessionServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            // .AddGrpcPolicies();
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();

            // ✅ Register Unit gRPC Client
            services.AddGrpcClient<UnitService.UnitServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUnitGrpcClient, UnitGrpcClient>();

            // ✅ Register Company gRPC Client         
            services.AddGrpcClient<CompanyService.CompanyServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICompanyGrpcClient, CompanyGrpcClient>();


            services.AddGrpcClient<MaintenanceJobService.MaintenanceJobServiceClient>(options =>
            {
                options.Address = new Uri(backGroundServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)

            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {

                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddGrpcClient<MaintenanceHangfireDeleteService.MaintenanceHangfireDeleteServiceClient>(options =>
           {
               options.Address = new Uri(backGroundServiceUrl);
           })
           .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)

           .ConfigurePrimaryHttpMessageHandler(() =>
           {
               return new HttpClientHandler
               {

                   ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
               };
           })
           .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
           .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IBackgroundServiceClient, BackgroundServiceClient>();

            // ✅ Register AssetSpecifications gRPC Client
            services.AddGrpcClient<AssetSpecificationService.AssetSpecificationServiceClient>(options =>
            {
                options.Address = new Uri(fixedassetmanagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IAssetSpecificationGrpcClient, AssetSpecificationGrpcClient>();

            // ✅ Register AllDepartment gRPC Client
            services.AddGrpcClient<DepartmentAllService.DepartmentAllServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IDepartmentAllGrpcClient, DepartmentAllGrpcClient>();


            return services;
        }
    }
}
