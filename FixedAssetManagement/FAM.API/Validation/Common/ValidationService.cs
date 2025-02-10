using FluentValidation;
using FAM.API.Validation.Locations;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;


namespace FAM.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateLocationCommand>, CreateLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateLocationCommand>, UpdateLocationCommandValidator>();

        
    }  
    }
}