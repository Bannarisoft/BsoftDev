
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
            return services;
        }
    }
}
