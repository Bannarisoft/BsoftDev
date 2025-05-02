using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SagaOrchestrator.Application.Orchestration;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Models;
using SagaOrchestrator.Application.Orchestration.Services.AssetServices;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Application.Orchestration.Services.UserServices;
using SagaOrchestrator.Application.StateMachines;
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
                // Register Saga
                x.AddSagaStateMachine<WorkOrderSchedulerStateMachine, WorkOrderSchedulerState>()
                    .InMemoryRepository(); // You can replace with MongoDbRepository or EF if needed

                // Register Event Consumers (for other workflows if any)
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
                   cfg.ConfigureEndpoints(context);
                });
            });


           
            return services;
        }
    }
}