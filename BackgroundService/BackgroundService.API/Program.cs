using BackgroundService.Infrastructure;
using BackgroundService.Application;
using BackgroundService.API.Configurations;
using BackgroundService.API;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration
.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables();


// Add services
builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Load configuration
var configuration = builder.Configuration;
builder.Services.AddInfrastructureServices(configuration);

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();
app.Run();
