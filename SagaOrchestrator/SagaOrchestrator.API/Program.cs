using MassTransit;
using Microsoft.OpenApi.Models;
using SagaOrchestrator.Infrastructure;
using SagaOrchestrator.Infrastructure.PollyResilience;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructureServices(builder.Configuration);
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
