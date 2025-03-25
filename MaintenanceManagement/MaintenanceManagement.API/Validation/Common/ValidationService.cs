using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.WorkCenter.Command.CreateWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using Core.Application.WorkCenter.Command.UpdateWorkCenter;
using FluentValidation;
using MaintenanceManagement.API.Validation.CostCenter;
using MaintenanceManagement.API.Validation.WorkCenter;

namespace MaintenanceManagement.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateCostCenterCommand>, CreateCostCenterCommandValidator>();
        services.AddScoped<IValidator<UpdateCostCenterCommand>, UpdateCostCenterCommandValidator>();
        services.AddScoped<IValidator<DeleteCostCenterCommand>, DeleteCostCenterCommandValidator>();
        services.AddScoped<IValidator<CreateWorkCenterCommand>, CreateWorkCenterCommandValidator>();
        services.AddScoped<IValidator<UpdateWorkCenterCommand>, UpdateWorkCenterCommandValidator>();
        services.AddScoped<IValidator<DeleteWorkCenterCommand>, DeleteWorkCenterCommandValidator>();
    }  
    }
}