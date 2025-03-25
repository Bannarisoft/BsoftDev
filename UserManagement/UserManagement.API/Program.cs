using Core.Application;
using UserManagement.Infrastructure;
using UserManagement.API.Validation.Common;
using UserManagement.API.Middleware;
using UserManagement.API.Configurations;
using Core.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    // If environment is null or empty, set default to "Development"
    if (string.IsNullOrWhiteSpace(environment))
    {      
        environment = "Development";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment, EnvironmentVariableTarget.Process);
    }

builder.Configuration    
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("settings/emailsetting.json", optional: false, reloadOnChange: true)
    .AddJsonFile("settings/smssetting.json", optional: false, reloadOnChange: true)    
    .AddJsonFile($"settings/serilogsetting.{environment}.json", optional: false, reloadOnChange: true) 
    .AddJsonFile("settings/jwtsetting.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();


// Configure Serilog
builder.Host.ConfigureSerilog(builder.Configuration); 

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);
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