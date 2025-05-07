using Core.Application;
using FAM.Infrastructure;
using FAM.API.Validation.Common;
using FAM.API.Configurations;
using MassTransit;
using Contracts.Events;
using FAM.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


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
//app.UseMiddleware<TokenValidationMiddleware>();
//app.UseMiddleware<FAM.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();
app.Run();

