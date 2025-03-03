using MassTransit;
using SagaOrchestrator.API.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddMassTransitHostedService();

// builder.Services.AddMassTransit(x =>
// {
//     x.AddConsumer<UserCreatedEventConsumer>();

//     x.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host("rabbitmq://localhost");

//         cfg.ReceiveEndpoint("user-created-queue", e =>
//         {
//             e.ConfigureConsumer<UserCreatedEventConsumer>(context);
//         });
//     });
// });

builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.Run();
