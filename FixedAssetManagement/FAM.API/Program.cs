using Core.Application;
using FAM.Infrastructure;
using FAM.API.Validation.Common;
using FAM.API.Configurations;
using MassTransit;
using Contracts.Events;
using FAM.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

//var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environment}.json");


// If environment is null or empty, set default to "Development"
if (string.IsNullOrWhiteSpace(environment))
{
    environment = "Development";
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment, EnvironmentVariableTarget.Process);

}

// Load configuration files based on the environment
builder.Configuration
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"settings/serilogsetting.{environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("settings/jwtsetting.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
// Configure Serilog
builder.Host.ConfigureSerilog(builder.Configuration);


// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Services);
builder.Services.AddSagaInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

// Configure MassTransit with RabbitMQ
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

// builder.Services.AddMassTransit(cfg =>
// {
//     cfg.AddConsumer<FixedAssetConsumer>();

//     cfg.UsingRabbitMq((context, config) =>
//     {
//         config.Host("rabbitmq://localhost", h =>
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });

//         config.ReceiveEndpoint("user-created-queue", e =>
//         {
//             e.ConfigureConsumer<FixedAssetConsumer>(context);
//         });
//     });
// });

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();
//app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<FAM.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();
app.Run();

