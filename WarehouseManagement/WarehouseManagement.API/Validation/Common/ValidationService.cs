using Core.Application.RackMaster.Command.CreateRackMaster;
using Core.Application.RackMaster.Command.DeleteRackMaster;
using Core.Application.RackMaster.Command.UpdateRackMaster;
using Core.Application.WarehouseMaster.Command.CreateWarehouseMaster;
using Core.Application.WarehouseMaster.Command.DeleteWarehouseMaster;
using Core.Application.WarehouseMaster.Command.UpdateWarehouseMaster;
using FluentValidation;
using WarehouseManagement.API.Validation.RackMaster;
using WarehouseManagement.API.Validation.WarehouseMaster;

namespace WarehouseManagement.API.Validation.Common
{
    public class ValidationService
    {
        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IValidator<CreateWarehouseMasterCommand>, CreateWarehouseMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateWarehouseMasterCommand>, UpdateWarehouseMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteWarehouseMasterCommand>, DeleteWareMasterCommandValidator>();
            services.AddScoped<IValidator<CreateRackMasterCommand>, CreateRackMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateRackMasterCommand>, UpdateRackMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteRackMasterCommand>, DeleteRackMasterCommandValidator>();
            
        }
    }
}