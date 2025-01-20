using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Core.Application;
using BSOFT.Infrastructure;
using BSOFT.API.Validation.Common;
using Serilog;
using MediatR;
using Core.Application.State.Commands.CreateState;
using Core.Domain.Entities;
using BSOFT.API;



var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging to MongoDB and console
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log to console for debugging
    .WriteTo.MongoDB("mongodb://192.168.1.126:27017/Bannari") // MongoDB connection string (adjust as needed)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging in the app

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


//Add layer dependency 
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
   
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();


var app = builder.Build();
/*  
 // Map endpoint to handle CreateStateCommand
app.MapPost("/state", async (
    CreateStateCommand request,
    IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var result = await mediator.Send(request, cancellationToken);

    
if (!result.IsSuccess)
    {
        return Results.BadRequest(result);
    }

    return Results.Created($"/states/{result.Data.Id}", result.Data);
});
 */

// Register LoggingMiddleware
app.UseMiddleware<BSOFT.Infrastructure.Logging.Middleware.LoggingMiddleware>(); 
//app.UseMiddleware<GlobalExceptionMiddleware>();// Register custom middleware
// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); 
}

app.UseHttpsRedirection();

app.UseRouting(); // Enable routing
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
