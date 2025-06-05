using Core.Application;
using UserManagement.Infrastructure;
using UserManagement.API.Validation.Common;
using UserManagement.API.Middleware;
using UserManagement.API.Configurations;
using UserManagement.Infrastructure.PollyResilience;
using UserManagement.API.GrpcServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;


var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Configuration

.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
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
builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddSagaInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Services);
builder.Services.AddHttpClientServices(); // Register HttpClient with Polly
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();

// Register gRPC
builder.Services.AddGrpc();

var app = builder.Build();
// builder.Services.AddScoped<SessionGrpcService>();


// Configure the HTTP request pipeline. 
//if (app.Environment.IsDevelopment())
//{
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
//}
app.UseHttpsRedirection();
app.UseRouting(); // Enable routing
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();

app.UseMiddleware<UserManagement.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DepartmentGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<SessionGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<UnitGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<CompanyGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<DepartmentAllGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<CityGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<StatesGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<CountryGrpcService>().EnableGrpcWeb();
    endpoints.MapControllers();
});

app.Run();