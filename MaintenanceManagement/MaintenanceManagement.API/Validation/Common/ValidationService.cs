using FluentValidation;

namespace MaintenanceManagement.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();

    }  
    }
}