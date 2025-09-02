using Core.Application;
using UserManagement.Infrastructure;
using UserManagement.API.Validation.Common;
using UserManagement.API.Middleware;
using UserManagement.API.Configurations;
using UserManagement.API.GrpcServices;
using MediatR;
using Core.Application.Common.Behaviors;
using MassTransit;



var builder = WebApplication.CreateBuilder(args);


// var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

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
// builder.Services.AddSagaInfrastructure(builder.Configuration);
// builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
// ⬇️ IMPORTANT: Avoid wiring the real bus in tests
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSagaInfrastructure(builder.Configuration);        // likely registers RabbitMQ MassTransit
    builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
}
else
{
    // Provide a single in-memory MassTransit bus for tests
    builder.Services.AddMassTransit(x =>
    {
        x.UsingInMemory((ctx, cfg) => { });
    });

    // If your infrastructure method also adds health checks for the bus,
    // and you see "masstransit-bus" duplicates again, you can optionally remove the MT health check here:
    // builder.Services.PostConfigure<HealthCheckServiceOptions>(o =>
    // {
    //     foreach (var reg in o.Registrations.Where(r => r.Name == "masstransit-bus").ToList())
    //         o.Registrations.Remove(reg);
    // });
}
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
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
// app.UseMiddleware<TokenValidationMiddleware>();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseMiddleware<TokenValidationMiddleware>();
}


app.UseMiddleware<UserManagement.Infrastructure.Logging.Middleware.LoggingMiddleware>();
app.UseAuthorization();
app.MapHealthChecks("/health").AllowAnonymous();

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
    endpoints.MapGrpcService<UserAllGrpcService>().EnableGrpcWeb();
    endpoints.MapGrpcService<GetorCreatelocationGrpService>().EnableGrpcWeb(); // PartyMaster City,State,Country Create/Get
    endpoints.MapGrpcService<UserUnitGrpcService>().EnableGrpcWeb();
    endpoints.MapControllers();
});

app.Run();
public partial class Program { }