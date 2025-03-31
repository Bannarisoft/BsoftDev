using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
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
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var rabbitMqConsumer = scope.ServiceProvider.GetRequiredService<IRabbitMqConsumer>();
    Task.Run(() => rabbitMqConsumer.StartConsuming()).ConfigureAwait(false);
}

app.Run();
