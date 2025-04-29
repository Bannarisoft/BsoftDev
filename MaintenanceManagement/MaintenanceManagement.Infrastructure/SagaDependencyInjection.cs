using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Consumers;
using MaintenanceManagement.Infrastructure.Persistence;
using MaintenanceManagement.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace MaintenanceManagement.Infrastructure
{
    public static class SagaDependencyInjection
    {
        public static IServiceCollection AddSagaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the OutboxMessage collection
            services.AddScoped<IMongoCollection<OutboxMessage>>(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                var collectionName = configuration["MongoDbSettings:OutboxCollectionName"] ?? "OutboxMessages";
                return database.GetCollection<OutboxMessage>(collectionName);
            });

            // Register your EventPublisher service
            services.AddScoped<IEventPublisher, EventPublisher>();

            // Configure MassTransit with RabbitMQ
            services.AddMassTransit(x =>
            {
                 x.AddConsumer<CreateNextSchedulerConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                     cfg.ReceiveEndpoint("create-next-scheduler-queue", e =>
                    {
                        e.ConfigureConsumer<CreateNextSchedulerConsumer>(context);
                    });
                });
            });

            // Ensure MassTransit background service is added
            services.AddMassTransitHostedService();

            return services;
        }
    }
}