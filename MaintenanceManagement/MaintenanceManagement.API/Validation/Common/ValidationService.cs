using Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.UpdateShiftMaster;
using FluentValidation;
using MaintenanceManagement.API.Validation.ShiftMaster;

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

    }  
    }
}