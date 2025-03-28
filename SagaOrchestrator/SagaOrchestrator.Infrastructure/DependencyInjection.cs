using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Services;
using SagaOrchestrator.Infrastructure.Services;

namespace SagaOrchestrator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IServiceCollection builder)
        {
            // Register OrchestratorService
            services.AddScoped<OrchestratorService>();

            // HttpClient Registration using HttpClientFactory
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5174");
            });
            services.AddHttpClient<IAssetService, AssetService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5194");
            });

            // Configure MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });


            // Register IPublishEndpoint from MassTransit
            services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

            return services;
        }
    }
}