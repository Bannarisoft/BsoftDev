using Core.Application;
using FAM.Infrastructure;
using FAM.API.Validation.Common;
using FAM.API.Configurations;
using MassTransit;
using Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging to MongoDB and console

// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Debug()
//     .WriteTo.Console() // Log to console for debugging
//     .WriteTo.MongoDB("mongodb://192.168.1.126:27017/FixedAsset") // MongoDB connection string (adjust as needed)
//     .WriteTo.MongoDB("mongodb://192.168.1.126:27017/FixedAsset", collectionName: "ApplicationLogs", restrictedToMinimumLevel: LogEventLevel.Warning)
//     .Enrich.FromLogContext()
//     .CreateLogger();

// builder.Host.UseSerilog(); // Use Serilog for logging in the app


// Load Serilog configuration from appsettings.json
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/serilogsetting.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/jwtsetting.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure Serilog
builder.Host.ConfigureSerilog();

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddConsumer<UserCreatedConsumer>();

    cfg.UsingRabbitMq((context, config) =>
    {
        config.Host("rabbitmq://localhost");

        config.ReceiveEndpoint("user-created-event-queue", ep =>
        {
            ep.ConfigureConsumeTopology = false;
            ep.Bind<UserCreatedEvent>();
            ep.ConfigureConsumer<UserCreatedConsumer>(context);
        });
    });
});

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
//Add layer dependency & Add services to the container.
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
app.UseMiddleware<FAM.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();
app.MapGet("/", () => "Fixed Asset Management Running");
app.Run();
