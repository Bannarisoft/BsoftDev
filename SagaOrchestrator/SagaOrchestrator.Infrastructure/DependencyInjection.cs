using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaOrchestrator.Application;
using SagaOrchestrator.Application.Orchestration;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Models;
using SagaOrchestrator.Application.Orchestration.Services.AssetServices;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Application.Orchestration.Services.UserServices;
using SagaOrchestrator.Infrastructure.Consumers;
using SagaOrchestrator.Infrastructure.Services.AssetServices;
using SagaOrchestrator.Infrastructure.Services.MaintenanceServices;
using SagaOrchestrator.Infrastructure.Services.UserServices;

namespace SagaOrchestrator.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // HttpClient Registration using HttpClientFactory
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5174");
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });
            services.AddHttpClient<IAssetService, AssetService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5194");
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });
            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                // client.BaseAddress = new Uri("http://localhost:5174");
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });


            // Register OrchestratorServices
            // services.AddScoped<OrchestratorService>();
            services.AddScoped<UserSagaService>();
            services.AddScoped<AssetSagaService>();
            services.AddScoped<DepartmentSagaService>();

            // Configure MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<UserAssetStateMachine, UserAssetState>()
                 .InMemoryRepository();
                x.AddSagaStateMachine<WorkOrderSchedulerStateMachine, WorkOrderSchedulerState>()
                .InMemoryRepository();
                // .MongoDbRepository(r =>
                //       {
                //           r.Connection = "mongodb://192.168.1.126:27017";
                //           r.DatabaseName = "saga_orchestrator_db";
                //       });


                x.AddConsumer<UserCreatedEventConsumer>();
                x.AddConsumer<AssetCreatedEventConsumer>();
                x.AddConsumer<SagaCompletedEventConsumer>();
                x.AddConsumer<DeleteUserCommandConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {

                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    // Automatically configure endpoints for sagas and consumers
                    cfg.ConfigureEndpoints(context);
                    cfg.ReceiveEndpoint("workorder-saga-queue", e =>
                    {
                        e.ConfigureSaga<WorkOrderSchedulerState>(context);
                    });
                

                });
            });


            // Register IPublishEndpoint from MassTransit
            // services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

            return services;
        }
    }
}