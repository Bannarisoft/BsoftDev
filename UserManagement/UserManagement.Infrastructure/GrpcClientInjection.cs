using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using GrpcServices.Maintenance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;
using UserManagement.Infrastructure.GrpcClients;

namespace UserManagement.Infrastructure
{
    public static class GrpcClientInjection
    {
        private static readonly HttpClientHandler GrpcHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            var maintenanceServiceUrl = configuration["GrpcSettings:MaintenanceServiceUrl"];

            // ✅ Register DepartmentValidation gRPC Client (Maintenance → User)
            services.AddGrpcClient<DepartmentValidationService.DepartmentValidationServiceClient>(options =>
            {
                options.Address = new Uri(maintenanceServiceUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IDepartmentValidationGrpcClient, DepartmentValidationGrpcClient>();

            return services;
        }
    }
}