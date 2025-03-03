using Core.Application;
using UserManagement.Infrastructure;
using UserManagement.API.Validation.Common;
using UserManagement.API.Middleware;
using UserManagement.API.Configurations;
using Core.Domain.Entities;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/emailsetting.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/smssetting.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/serilogsetting.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/jwtsetting.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();


// Configure Serilog
builder.Host.ConfigureSerilog();

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);

builder.Services.AddMassTransit(cfg =>
{
    cfg.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddMassTransitHostedService();

// builder.Services.AddMassTransit(busConfig =>
// {
//     busConfig.SetKebabCaseEndpointNameFormatter();

//     busConfig.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.Host("localhost", "/", h =>
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });

//         cfg.ConfigureEndpoints(context);
//     });
// });

// Register Services
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Services);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

var app = builder.Build();
// Configure the HTTP request pipeline. 
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
//}
app.UseHttpsRedirection();
app.UseRouting(); // Enable routing
app.UseCors();// Enable CORS
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<UserManagement.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();
app.Run();