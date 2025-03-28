using Core.Application;
using MaintenanceManagement.Infrastructure;
using MaintenanceManagement.API.Configurations;
using MaintenanceManagement.API.Validation.Common;
using MassTransit;
using MaintenanceManagement.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

// MassTransit Configuration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DepartmentCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
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
app.UseMiddleware<MaintenanceManagement.Infrastructure.Logging.Middleware.LoggingMiddleware>(); 
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();

app.Run();
