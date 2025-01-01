using MediatR;
using Core.Application;
using BSOFT.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;
using Core.Application.Country.Commands.CreateCountry;
using BSOFT.API.Validation.Common.Country;
using BSOFT.API.Validation.Common;
using Core.Application.Country.Commands.UpdateCountry;
using BSOFT.API.Validation.Common.Users;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUser;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using BSOFT.API.Validation.Common.RoleEntitlements;
using Core.Application.Modules.Commands.CreateModule;
using Core.Application.Modules.Commands.UpdateModule;
using BSOFT.API.Validation.Common.Module;
using Core.Application.Entity.Commands.CreateEntity;
using BSOFT.API.Validation.Common.Entity;
using Core.Application.Entity.Commands.UpdateEntity;
using Core.Application.Units.Commands.CreateUnit;
using BSOFT.API.Validation.Common.Unit;
using Core.Application.Units.Commands.UpdateUnit;
using BSOFT.API.Validation.Common.Unit.BSOFT.API.Validation.Common.Unit;
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

// //Validation
// builder.Services.AddMediatR(cfg =>
// {
//     cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
//     cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
// });

//FLuent Validation
builder.Services.AddScoped<MaxLengthProvider>();
builder.Services.AddScoped<IValidator<CreateCountryCommand>, CreateCountryCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCountryCommand>, UpdateCountryCommandValidator>();
builder.Services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
builder.Services.AddScoped<IValidator<CreateRoleEntitlementCommand>, CreateRoleEntitlementCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateRoleEntitlementCommand>, UpdateRoleEntitlementCommandValidator>();
builder.Services.AddScoped<IValidator<CreateModuleCommand>, CreateModuleCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateModuleCommand>, UpdateModuleCommandValidator>();
builder.Services.AddScoped<IValidator<CreateEntityCommand>, CreateEntityCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateEntityCommand>, UpdateEntityCommandValidator>();
builder.Services.AddScoped<IValidator<CreateUnitCommand>, CreateUnitCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateUnitCommand>, UpdateUnitCommandValidator>();
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
