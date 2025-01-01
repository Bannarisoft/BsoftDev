using MediatR;
using BSOFT.API.Behaviors;
using Core.Application;
using BSOFT.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;
using Core.Application.Country.Commands.CreateCountry;
using BSOFT.API.Validation.Common.Country;
using BSOFT.API.Validation.Common;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Departments.Commands.CreateDepartment;
using BSOFT.API.Validation.Common.Department;
using Core.Application.Departments.Commands.UpdateDepartment;   
using BSOFT.API.Validation.Common.UserRole;
using Core.Application.UserRole.Commands.CreateRole; 
using Core.Application.UserRole.Commands.UpdateRole;

var builder = WebApplication.CreateBuilder(args);

// Validate JWT Key
string? jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key must be at least 32 characters long.");
}

//Validation
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

//FLuent Validation
builder.Services.AddScoped<MaxLengthProvider>();
builder.Services.AddScoped<IValidator<CreateCountryCommand>, CreateCountryCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCountryCommand>, UpdateCountryCommandValidator>();

builder.Services.AddScoped<IValidator<CreateDepartmentCommand>, CreateDepartmentCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateDepartmentCommand>, UpdateDepartmentCommandValidator>();

builder.Services.AddScoped<IValidator<CreateRoleCommand>, CreateRoleCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();

// Add services to the container.

//Add layer dependency 
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
   
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
    

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
