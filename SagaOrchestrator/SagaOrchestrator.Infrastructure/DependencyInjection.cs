using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Email;
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

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
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

            });

            // Register OrchestratorService
            services.AddScoped<OrchestratorService>();
            services.AddScoped<DepartmentSagaService>();            
            //services.Configure<MailSettings>(configuration.GetSection("EmailSettings"));     
           var emailSettings = new MailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);        

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
                      //.InMemoryRepository();

             
              
              .MongoDbRepository(r =>
                    {
                        r.Connection = "mongodb://192.168.1.126:27017";
                        r.DatabaseName = "saga_orchestrator_db";
                    });
              

              //   x.AddConsumer<UserCreatedEventConsumer>();
              //   x.AddConsumer<AssetCreatedEventConsumer>();
              //   x.AddConsumer<SagaCompletedEventConsumer>();
                x.AddConsumer<EmailEventConsumer>();
              x.UsingRabbitMq((context, cfg) =>
              {
               
                  cfg.Host("localhost", "/", h =>
                  {
                      h.Username("guest");
                      h.Password("guest");
                  });
                  // Automatically configure endpoints for sagas and consumers
                  cfg.ConfigureEndpoints(context);

              

                
                cfg.ReceiveEndpoint("email-queue", e =>
                {
                    e.ConfigureConsumer<EmailEventConsumer>(context);
                });
               
              });
          });


            // Register IPublishEndpoint from MassTransit
            services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

            return services;
        }
    }
}