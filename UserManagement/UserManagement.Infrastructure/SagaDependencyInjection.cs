using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UserManagement.Infrastructure.Persistence;
using UserManagement.Infrastructure.Services;

namespace UserManagement.Infrastructure
{
    public static class SagaDependencyInjection
    {
        public static IServiceCollection AddSagaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MongoDB Client
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("MongoDb");
                return new MongoClient(connectionString);
            });
            // Register the MongoDB Database
            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var databaseName = configuration["MongoDbSettings:DatabaseName"];
                return client.GetDatabase(databaseName);
            });

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
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });

            // Ensure MassTransit background service is added
            services.AddMassTransitHostedService();

            return services;
        }
    }
}