using Core.Application.WarehouseMaster.Command.CreateWarehouseMaster;
using Core.Application.WarehouseMaster.Command.UpdateWarehouseMaster;
using FluentValidation;
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
            
        }
    }
}