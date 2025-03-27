using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Contracts.Events;
using SagaOrchestrator.Application;

var builder = WebApplication.CreateBuilder(args);

// Add MassTransit & RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<UserAssetStateMachine, UserAssetState>().InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        
        cfg.ReceiveEndpoint("saga-orchestrator-queue", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind<UserCreatedEvent>();
            e.ConfigureSaga<UserAssetState>(context);
        });

        cfg.ConfigureEndpoints(context);  // Auto-configuration of endpoints
    });
});


// Register Controllers
builder.Services.AddControllers();

// Enable Logging for MassTransit (Optional but useful for debugging)
LogContext.ConfigureCurrentLogContext();

var app = builder.Build();

// Middleware Setup
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();

app.Run();
