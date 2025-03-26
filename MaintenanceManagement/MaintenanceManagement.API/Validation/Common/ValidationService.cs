using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.WorkCenter.Command.CreateWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using Core.Application.WorkCenter.Command.UpdateWorkCenter;

using Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasterDetailCQRS.Commands.CreateShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.DeleteShiftMasterDetail;
using Core.Application.ShiftMasterDetailCQRS.Commands.UpdateShiftMasterDetail;
using FluentValidation;
using MaintenanceManagement.API.Validation.CostCenter;
using MaintenanceManagement.API.Validation.WorkCenter;
using MaintenanceManagement.API.Validation.MachineGroup;
using MaintenanceManagement.API.Validation.MiscMaster;
using MaintenanceManagement.API.Validation.MiscTypeMaster;
using MaintenanceManagement.API.Validation.ShiftMaster;
using MaintenanceManagement.API.Validation.ShiftMasterDetail;

namespace MaintenanceManagement.API.Validation.Common
{
    public class ValidationService
    {

        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IValidator<CreateMachineGroupCommand>, CreateMachineGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateMachineGroupCommand>, UpdateMachineGroupCommandValidator>();   
            services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>(); 
        services.AddScoped<IValidator<CreateShiftMasterCommand>, CreateShiftMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateShiftMasterCommand>, UpdateShiftMasterCommandValidator>();
        services.AddScoped<IValidator<DeleteShiftMasterCommand>, DeleteShiftMasterCommandValidator>();
        services.AddScoped<IValidator<CreateShiftMasterDetailCommand>, CreateShiftMasterDetailCommandValidator>();
        services.AddScoped<IValidator<UpdateShiftMasterDetailCommand>, UpdateShiftMasterDetailCommandValidator>();
        services.AddScoped<IValidator<DeleteShiftMasterDetailCommand>, DeleteShiftMasterDetailCommandValidator>();
 		services.AddScoped<IValidator<CreateCostCenterCommand>, CreateCostCenterCommandValidator>();
        services.AddScoped<IValidator<UpdateCostCenterCommand>, UpdateCostCenterCommandValidator>();
        services.AddScoped<IValidator<DeleteCostCenterCommand>, DeleteCostCenterCommandValidator>();
        services.AddScoped<IValidator<CreateWorkCenterCommand>, CreateWorkCenterCommandValidator>();
        services.AddScoped<IValidator<UpdateWorkCenterCommand>, UpdateWorkCenterCommandValidator>();
        services.AddScoped<IValidator<DeleteWorkCenterCommand>, DeleteWorkCenterCommandValidator>();



        }  
    }
}