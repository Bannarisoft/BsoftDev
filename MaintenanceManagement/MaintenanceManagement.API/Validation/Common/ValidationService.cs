using Core.Application.MachineGroup.Command.CreateMachineGroup;
using Core.Application.MachineGroup.Command.UpdateMachineGroup;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;

using FluentValidation;
using MaintenanceManagement.API.Validation.MachineGroup;
using MaintenanceManagement.API.Validation.MiscMaster;
using MaintenanceManagement.API.Validation.MiscTypeMaster;

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


        }  
    }
}