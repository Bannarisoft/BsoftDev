using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Application;
using Core.Domain.Entities;
using BSOFT.Infrastructure;
using BSOFT.API.Validation.Common;
using Serilog;
using BSOFT.API.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
// builder.Host.UseSerilog((context, services, configuration) =>
// {
//     configuration
//         .ReadFrom.Configuration(context.Configuration)
//         .ReadFrom.Services(services)
//         .Enrich.FromLogContext();
// });

// Add validation services
var validationService = new ValidationService();
validationService.AddValidationServices(builder.Services);

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Add Authentication and configure JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Ensure the token's `iss` matches
        ValidateAudience = true, // Ensure the token's `aud` matches
        ValidateLifetime = true, // Ensure the token hasn't expired
        ValidateIssuerSigningKey = true, // Ensure the signature is valid
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Configure Serilog
// builder.Host.UseSerilog();

//Add layer dependency 
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
   
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Use Global Exception Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Register LoggingMiddleware
app.UseMiddleware<BSOFT.Infrastructure.Logging.Middleware.LoggingMiddleware>();    

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Handle Serilog lifecycle
try
{
    Log.Information("Starting the application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
