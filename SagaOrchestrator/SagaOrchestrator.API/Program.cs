using MassTransit;
using Microsoft.OpenApi.Models;
using SagaOrchestrator.Infrastructure;
using SagaOrchestrator.Infrastructure.PollyResilience;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHttpClient<IUserService, UserService>(client =>
// {
//     client.BaseAddress = new Uri("http://localhost:5174");
// });
// builder.Services.AddHttpClient<IAssetService, AssetService>(client =>
// {
//     client.BaseAddress = new Uri("http://localhost:5194");
// });
// builder.Services.AddMassTransit(x =>
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



// // Register OrchestratorService
// builder.Services.AddScoped<OrchestratorService>();

// // Register IPublishEndpoint from MassTransit
// builder.Services.AddScoped<IPublishEndpoint>(provider => provider.GetRequiredService<IBus>());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructureServices();
builder.Services.AddHttpClientServices(); // Register HttpClient with Polly
// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Saga Orchestrator API", Version = "v1" });
});

// Enable Logging for MassTransit (Optional but useful for debugging)
LogContext.ConfigureCurrentLogContext();

var app = builder.Build();

// Middleware Setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Saga Orchestrator API V1");
    });
}
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapControllers();

app.Run();
