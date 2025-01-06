using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MediatR;
using Core.Application;
using Core.Domain.Entities;
using BSOFT.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;
using Core.Application.Country.Commands.CreateCountry;
using BSOFT.API.Validation.Common.Country;
using BSOFT.API.Validation.Common;
using Core.Application.Country.Commands.UpdateCountry;
using BSOFT.API.Validation.Common.State;
using Core.Application.State.Commands.UpdateState;
using Core.Application.State.Commands.CreateState;
using BSOFT.API.Validation.Common.City;
using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.UpdateCity;using BSOFT.API.Validation.Common.Users;
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
using Core.Application.Companies.Commands.CreateCompany;
using Core.Application.Companies.Queries.GetCompanies;
using BSOFT.API.Validation.Common.Companies;
using Core.Application.Companies.Commands.UpdateCompany;
using Core.Application.Departments.Commands.CreateDepartment;
using BSOFT.API.Validation.Common.Department;
using Core.Application.Departments.Commands.UpdateDepartment;   
using BSOFT.API.Validation.Common.UserRole;
using Core.Application.UserRole.Commands.CreateRole; 
using Core.Application.UserRole.Commands.UpdateRole;
using Core.Application.Divisions.Commands.CreateDivision;
using BSOFT.API.Validation.Common.Divisions;
using Core.Application.Divisions.Commands.UpdateDivision;

var builder = WebApplication.CreateBuilder(args);

// // Validate JWT Key
// string? jwtKey = builder.Configuration["Jwt:Key"];
// if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
// {
//     throw new InvalidOperationException("Jwt:Key must be at least 32 characters long.");
// }


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
builder.Services.AddScoped<IValidator<CreateCompanyCommand>, CreateCompanyCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCompanyCommand>, UpdateCompanyCommandValidator>();
builder.Services.AddScoped<IValidator<CreateDivisionCommand>, CreateDivisionCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateDivisionCommand>, UpdateDivisionCommandValidator>();
builder.Services.AddScoped<IValidator<CreateStateCommand>, CreateStateCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateStateCommand>, UpdateStateCommandValidator>();

builder.Services.AddScoped<IValidator<CreateCityCommand>, CreateCityCommandValidator>();
builder.Services.AddScoped<IValidator<UpdateCityCommand>, UpdateCityCommandValidator>();
// Add services to the container.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//         ValidateIssuer = false,
//         ValidateAudience = false,
//         RoleClaimType = ClaimTypes.Role // Ensure roles are validated
//     };
// });

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
