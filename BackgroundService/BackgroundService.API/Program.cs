using BackgroundService.Infrastructure;
using BackgroundService.Application;
using BackgroundService.API.Configurations;
using BackgroundService.API;
using BackgroundService.API.GrpcServices;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure.Services;
using BackgroundService.API.Validation.Common;
using MediatR;
using BackgroundService.Application.Notification.Common.Behaviors;
using BackgroundService.Application.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?? "Development";

builder.Configuration
.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables();

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);
// Add services
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();


// Load configuration
builder.Services.AddGrpc();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Enable Swagger in Development
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();   
    app.UseDeveloperExceptionPage();
//}

app.UseHttpsRedirection();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseRouting();
app.UseCors("AllowAll");
app.UseMiddleware<BackgroundService.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<MaintenanceJobGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<MaintenanceHangfireRemoveGrpcService>().EnableGrpcWeb();
    endpoints.MapControllers();
});

// app.MapControllers();
app.ConfigureHangfireDashboard();
app.Run();
