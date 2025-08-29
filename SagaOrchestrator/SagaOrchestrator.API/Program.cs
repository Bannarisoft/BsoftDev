using System.Data;
using System.Data.SqlClient;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

// Health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

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
// ✅ Map the exact probe paths
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    // if you added dependency checks tagged "ready", filter by that:
    // Predicate = r => r.Tags.Contains("ready")
    Predicate = r => r.Tags.Contains("ready")
});
// keep /health too if you want a general check
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => true });
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
// app.MapControllers();

app.Run();
