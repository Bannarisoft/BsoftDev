
using Contracts.Interfaces.External.IInvetoryManagement;
using Contracts.Interfaces.External.IUser;
using GrpcServices.UserManagement;
using Inventory.Grpc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;
using WarehouseManagement.Infrastructure.GrpcClients;

namespace WarehouseManagement.Infrastructure
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
            var inventoryManagementUrl = configuration["GrpcSettings:InventoryManagementUrl"];

            // ✅ Register Session gRPC Client
            services.AddGrpcClient<SessionService.SessionServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();

            services.AddGrpcClient<MiscMasterService.MiscMasterServiceClient>(options =>
            {
                options.Address = new Uri(inventoryManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IMiscMasterGrpcClient, InventoryMiscMasterGrpcClient>();


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



             // ✅ Register  Invetory ItemGroup gRPC Client
            services.AddGrpcClient<InventoryItemGroupService.InventoryItemGroupServiceClient>(options =>
            {
                options.Address = new Uri(inventoryManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IItemGroupGrpcClient, InventoryItemGroupGrpcClient>();


            // ✅ Register  Invetory ItemGroup gRPC Client
            services.AddGrpcClient<InventoryUOMService.InventoryUOMServiceClient>(options =>
            {
                options.Address = new Uri(inventoryManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUOMGrpcClient,  InventoryUOMGrpcClient>();

               // ✅ Register City gRPC Client
            services.AddGrpcClient<CityService.CityServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICityGrpcClient,CityGrpcClient>();
        // ✅ Register State gRPC Client
              services.AddGrpcClient<StateService.StateServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IStatesGrpcClient,StateGrpcClient>();

             // ✅ Register Country gRPC Client
              services.AddGrpcClient<CountryService.CountryServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICountryGrpcClient,CountryGrpcClient>();


            return services;
            


        }
    }
}
