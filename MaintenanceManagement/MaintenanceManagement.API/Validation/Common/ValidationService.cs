using Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasterDetailCQRS.Commands.CreateShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.DeleteShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.UpdateShiftMasterDetail;
using FluentValidation;
using MaintenanceManagement.API.Validation.ShiftMaster;
using MaintenanceManagement.API.Validation.ShiftMasterDetail;

namespace MaintenanceManagement.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateShiftMasterCommand>, CreateShiftMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateShiftMasterCommand>, UpdateShiftMasterCommandValidator>();
        services.AddScoped<IValidator<DeleteShiftMasterCommand>, DeleteShiftMasterCommandValidator>();
        services.AddScoped<IValidator<CreateShiftMasterDetailCommand>, CreateShiftMasterDetailCommandValidator>();
        services.AddScoped<IValidator<UpdateShiftMasterDetailCommand>, UpdateShiftMasterDetailCommandValidator>();
        services.AddScoped<IValidator<DeleteShiftMasterDetailCommand>, DeleteShiftMasterDetailCommandValidator>();

    }  
    }
}