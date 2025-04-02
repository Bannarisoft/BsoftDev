using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaOrchestrator.Application;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Services;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Infrastructure.Consumers;
using SagaOrchestrator.Infrastructure.Services;
using SagaOrchestrator.Infrastructure.Services.MaintenanceServices;

namespace SagaOrchestrator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {

            // HttpClient Registration using HttpClientFactory
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5174");
            });
            services.AddHttpClient<IAssetService, AssetService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5194");
            });
            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5293");  // URL of MaintenanceService
            });

            // Register OrchestratorService
            services.AddScoped<OrchestratorService>();
            services.AddScoped<DepartmentSagaService>();


            // Configure MassTransit with RabbitMQ
            // services.AddMassTransit(x =>
            // {
            //     x.UsingRabbitMq((context, cfg) =>
            //     {
            //         cfg.Host("localhost", "/", h =>
            //         {
            //             h.Username("guest");
            //             h.Password("guest");
            //         });
            //     });
            // });

            services.AddMassTransit(x =>
          {
              x.AddSagaStateMachine<UserAssetStateMachine, UserAssetState>()
                  .InMemoryRepository();

              x.AddConsumer<UserCreatedEventConsumer>();
              x.AddConsumer<AssetCreatedEventConsumer>();
              x.AddConsumer<SagaCompletedEventConsumer>();

              x.UsingRabbitMq((context, cfg) =>
              {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("user-created-queue", e =>
                {
                    e.ConfigureConsumer<UserCreatedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("asset-created-queue", e =>
                {
                    e.ConfigureConsumer<AssetCreatedEventConsumer>(context);
                });

                cfg.ReceiveEndpoint("saga-completed-queue", e =>
                {
                    e.ConfigureConsumer<SagaCompletedEventConsumer>(context);
                });
              });
          });


            // Register IPublishEndpoint from MassTransit
            services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

            return services;
        }
    }
}