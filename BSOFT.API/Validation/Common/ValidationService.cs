using FluentValidation;
using BSOFT.API.Validation.Country;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands.UpdateCountry;
using BSOFT.API.Validation.State;
using Core.Application.State.Commands.CreateState;
using Core.Application.State.Commands.UpdateState;
using BSOFT.API.Validation.City;
using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.UpdateCity;
using BSOFT.API.Validation.Users;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUser;
using BSOFT.API.Validation.RoleEntitlements;
using Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement;
using Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement;
using BSOFT.API.Validation.Module;
using Core.Application.Modules.Commands.CreateModule;
using Core.Application.Modules.Commands.UpdateModule;
using BSOFT.API.Validation.Entity;
using Core.Application.Entity.Commands.CreateEntity;
using Core.Application.Entity.Commands.UpdateEntity;
using BSOFT.API.Validation.Unit;
using Core.Application.Units.Commands.CreateUnit;
using Core.Application.Units.Commands.UpdateUnit;
using BSOFT.API.Validation.Department;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Application.Departments.Commands.UpdateDepartment;
using BSOFT.API.Validation.UserRole;
using Core.Application.UserRole.Commands.CreateRole;
using Core.Application.UserRole.Commands.UpdateRole;
using BSOFT.API.Validation.Divisions;
using Core.Application.Divisions.Commands.CreateDivision;
using Core.Application.Divisions.Commands.UpdateDivision;
using BSOFT.API.Validation.Companies;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Application.Companies.Commands.UpdateCompany;
using BSOFT.API.Validation.Unit.BSOFT.API.Validation.Unit;
using Core.Application.Users.Commands.UpdateFirstTimeUserPassword;
using Core.Application.Users.Commands.ChangeUserPassword;
using Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule;
using BSOFT.API.Validation.PasswordComplexityrule;
using Core.Application.UserLogin.Commands.UserLogin;
using BSOFT.API.Validation.UserLogin;
using Microsoft.Extensions.DependencyInjection;
using Core.Application.PasswordComplexityRule.Commands.UpdatePasswordComplexityRule;
using Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings;
using BSOFT.API.Validation.AdminSecuritySettings;
using Core.Application.AdminSecuritySettings.Commands.UpdateAdminSecuritySettings;
using Core.Application.CompanySettings.Commands.CreateCompanySettings;
using BSOFT.API.Validation.CompanySettings;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using BSOFT.API.Validation.FinancialYear;
using Core.Application.FinancialYear.Command.UpdateFinancialYear;
namespace BSOFT.API.Validation.Common
{
    public class ValidationService
    {
       public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateCountryCommand>, CreateCountryCommandValidator>();
        services.AddScoped<IValidator<UpdateCountryCommand>, UpdateCountryCommandValidator>();
        services.AddScoped<IValidator<CreateUserCommand>, CreateUserCommandValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
        services.AddScoped<IValidator<CreateRoleEntitlementCommand>, CreateRoleEntitlementCommandValidator>();
        services.AddScoped<IValidator<UpdateRoleEntitlementCommand>, UpdateRoleEntitlementCommandValidator>();
        services.AddScoped<IValidator<CreateModuleCommand>, CreateModuleCommandValidator>();
        services.AddScoped<IValidator<UpdateModuleCommand>, UpdateModuleCommandValidator>();
        services.AddScoped<IValidator<CreateEntityCommand>, CreateEntityCommandValidator>();
        services.AddScoped<IValidator<UpdateEntityCommand>, UpdateEntityCommandValidator>();
        services.AddScoped<IValidator<CreateUnitCommand>, CreateUnitCommandValidator>();
        services.AddScoped<IValidator<UpdateUnitCommand>, UpdateUnitCommandValidator>();
        services.AddScoped<IValidator<CreateCompanyCommand>, CreateCompanyCommandValidator>();
        services.AddScoped<IValidator<UpdateCompanyCommand>, UpdateCompanyCommandValidator>();
        services.AddScoped<IValidator<CreateDepartmentCommand>, CreateDepartmentCommandValidator>();
        services.AddScoped<IValidator<UpdateDepartmentCommand>, UpdateDepartmentCommandValidator>();
        services.AddScoped<IValidator<CreateDivisionCommand>, CreateDivisionCommandValidator>();
        services.AddScoped<IValidator<UpdateDivisionCommand>, UpdateDivisionCommandValidator>();
        services.AddScoped<IValidator<CreateStateCommand>, CreateStateCommandValidator>();
        services.AddScoped<IValidator<UpdateStateCommand>, UpdateStateCommandValidator>();
        services.AddScoped<IValidator<CreateCityCommand>, CreateCityCommandValidator>();
        services.AddScoped<IValidator<UpdateCityCommand>, UpdateCityCommandValidator>();
        services.AddScoped<IValidator<CreateRoleCommand>, CreateRoleCommandValidator>();
        services.AddScoped<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
        services.AddScoped<IValidator<FirstTimeUserPasswordCommand>,PasswordChangeCommandValidator>();
        services.AddScoped<IValidator<ChangeUserPasswordCommand>,ExistingUserPasswordChangeCommandValidator>();

 		services.AddScoped<IValidator<CreatePasswordComplexityRuleCommand>, CreatePasswordComplexityRuleCommandValidator>();
        services.AddScoped<IValidator<UserLoginCommand>,UserLoginCommandValidator>();
        services.AddScoped<IValidator<UpdatePasswordComplexityRuleCommand>, UpdatePasswordComplexityRuleCommandValidator>();
        services.AddScoped<IValidator<CreateAdminSecuritySettingsCommand>, CreateAdminSecuritySettingsCommandValidator>(); 
        services.AddScoped<IValidator<UpdateAdminSecuritySettingsCommand> ,UpdateAdminSecuritySettingsCommandValidator>();  
        services.AddScoped<IValidator<FirstTimeUserPasswordCommand>,PasswordChangeCommandValidator>();
        services.AddScoped<IValidator<CreateCompanySettingsCommand>, CreateCompanySettingsCommandValidator>();
        services.AddScoped<IValidator<CreateFinancialYearCommand>, CreateFinancialYearCommandValidator>();
        services.AddScoped<IValidator<UpdateFinancialYearCommand>, UpdateFinancialYearCommandValidator>();
        
         }  
    }
}