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
using Microsoft.OpenApi.Models;
using BSOFT.API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
});

//Add layer dependency 
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration,builder.Services);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
// Add controllers with a global authorization policy

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}); 


builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
});


var app = builder.Build();
 


// Register LoggingMiddleware
app.UseMiddleware<BSOFT.Infrastructure.Logging.Middleware.LoggingMiddleware>(); 

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); 
}

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


app.UseHttpsRedirection();
app.UseRouting(); // Enable routing
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();

