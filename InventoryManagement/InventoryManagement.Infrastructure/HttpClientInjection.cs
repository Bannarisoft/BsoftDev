
using Contracts.Interfaces.External.IParty;
using Contracts.Interfaces.External.IUser;
using Contracts.Interfaces.External.IWarehouse;
using GrpcServices.Party.Party;
using GrpcServices.UserManagement;
using GrpcServices.Warehouse.Bin;
using GrpcServices.Warehouse.Rack;
using GrpcServices.Warehouse.Warehouse;
using InventoryManagement.Infrastructure.GrpcClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace InventoryManagement.Infrastructure
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
            var partyManagementUrl = configuration["GrpcSettings:PartyManagementUrl"];
            var warehouseManagementUrl = configuration["GrpcSettings:WarehouseManagementUrl"];

            
            // ✅ Register Session gRPC Client
            services.AddGrpcClient<SessionService.SessionServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();
            //unit grpc
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

            //party grpc
            services.AddGrpcClient<PartyService.PartyServiceClient>(options =>
            {
                options.Address = new Uri(partyManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IPartyGrpcClient, PartyGrpcClient>();
            //country grpc
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
             //Rack grpc
            services.AddGrpcClient<RackService.RackServiceClient>(options =>
            {
                options.Address = new Uri(warehouseManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IRackGrpcClient, RackGrpcClient>();
             //Bin grpc
            services.AddGrpcClient<BinService.BinServiceClient>(options =>
            {
                options.Address = new Uri(warehouseManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IBinGrpcClient, BinGrpcClient>();
             //Warehouse grpc
            services.AddGrpcClient<WarehouseService.WarehouseServiceClient>(options =>
            {
                options.Address = new Uri(warehouseManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IWarehouseGrpcClient, WarehouseGrpcClient>();
            


            return services;
        }
    }
}
