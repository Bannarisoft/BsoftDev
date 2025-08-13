using Contracts.Events.Notifications.WorkOrder;
using Contracts.Events.Notifications.WorkOrder.Sms;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;
using SagaOrchestrator.Application.Orchestration.Interfaces.IMaintenance;
using SagaOrchestrator.Application.Orchestration.Interfaces.IUsers;
using SagaOrchestrator.Application.Orchestration.Models;
using SagaOrchestrator.Application.Orchestration.Models.Notifications;
using SagaOrchestrator.Application.Orchestration.Models.PreventiveSchedule;
using SagaOrchestrator.Application.Orchestration.Models.Workflow;
using SagaOrchestrator.Application.Orchestration.Services.AssetServices;
using SagaOrchestrator.Application.Orchestration.Services.MaintenanceServices;
using SagaOrchestrator.Application.Orchestration.Services.UserServices;
using SagaOrchestrator.Application.StateMachines;
using SagaOrchestrator.Application.StateMachines.Notification;
using SagaOrchestrator.Application.StateMachines.Workflow;
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
            // ✅ HttpClient configurations
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });
            services.AddHttpClient<IAssetService, AssetService>(client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });
            services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.126:81");
            });

              // MongoDB Context
         var mongoConn = configuration.GetConnectionString("MongoDbConnectionString");
        var mongoDb   = configuration["MongoDb:DatabaseName"];

        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
        services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDb));

            // ✅ Domain Services
            services.AddScoped<UserSagaService>();
            services.AddScoped<AssetSagaService>();
            services.AddScoped<DepartmentSagaService>();

            // ✅ MassTransit setup
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // ✅ Saga registrations
                x.AddSagaStateMachine<WorkOrderSchedulerStateMachine, WorkOrderSchedulerState>().MongoDbRepository(r =>
                {
                    
                    r.Connection    = mongoConn;
                    r.DatabaseName  = mongoDb;
                    r.CollectionName = "preventive_create_state"; 

                });
                x.AddSagaStateMachine<PreventiveSchedulerStateMachine, PreventiveSchedulerState>().MongoDbRepository(r =>
                {
                    
                    r.Connection    = mongoConn;
                    r.DatabaseName  = mongoDb;
                    r.CollectionName = "preventive_create_state"; 

                });
                x.AddSagaStateMachine<PreventiveSchedulerUpdateStateMachine, PreventiveUpdateState>().MongoDbRepository(r =>
                {
                    
                    r.Connection    = mongoConn;
                    r.DatabaseName  = mongoDb;
                    r.CollectionName = "preventive_update_state"; 

                });

                // ✅ Register WorkOrderNotificationState saga and activity
                x.AddSagaStateMachine<WorkOrderNotificationState, NotificationWorkOrder>()
                    .InMemoryRepository();
                x.AddSagaStateMachine<ApprovalRequestStateMachine, ApprovalRequestState>().InMemoryRepository();

                x.AddConsumer<UserCreatedEventConsumer>();
                x.AddConsumer<AssetCreatedEventConsumer>();
                x.AddConsumer<SagaCompletedEventConsumer>();
                x.AddConsumer<DeleteUserCommandConsumer>();

                // ✅ Required for ResolveAndPublishNotificationActivity
                x.AddRequestClient<SendNotificationInternalCommand>();

                // ✅ Register the activity used inside WorkOrderNotificationState
                x.AddScoped<ResolveAndPublishNotificationActivity>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // ✅ Dedicated endpoint for saga
                    cfg.ReceiveEndpoint("workorder-notification-saga", e =>
                    {
                        e.ConfigureSaga<NotificationWorkOrder>(context); // automatic wiring
                        e.Bind<WorkOrderCreatedEvent>();
                    });

                    // ✅ Configure all endpoints (consumers/sagas)
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
