using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Contracts.Events;
using SagaOrchestrator.Application; // Ensure correct reference

var builder = WebApplication.CreateBuilder(args);

// Add MassTransit & RabbitMQ
builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    // Register the Saga State Machine and MongoDB Repository
    cfg.AddSagaStateMachine<UserAssetStateMachine, UserAssetState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://localhost:27017";
            r.DatabaseName = "saga_state";
        });

    cfg.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost");

        config.ReceiveEndpoint("user-created-event-queue", ep =>
        {
            ep.ConfigureConsumeTopology = false;
            ep.Bind<UserCreatedEvent>();
            ep.ConfigureSaga<UserAssetState>(context);
        });

        config.ConfigureEndpoints(context);
    });
});

// Register Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware Setup
app.UseRouting();
app.MapControllers();

app.Run();
