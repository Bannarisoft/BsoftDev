using Core.Application;
using MaintenanceManagement.Infrastructure;
using MaintenanceManagement.API.Configurations;
using MaintenanceManagement.API.Validation.Common;
using MaintenanceManagement.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ;
builder.Configuration

.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
.AddJsonFile($"settings/serilogsetting.{environment}.json", optional: false, reloadOnChange: true)
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
// builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddSagaInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Services);
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();

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
//app.UseMiddleware<TokenValidationMiddleware>();
//app.UseMiddleware<MaintenanceManagement.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.ConfigureHangfireDashboard();

app.Run();


