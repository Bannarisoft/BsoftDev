using FluentValidation;
using FAM.API.Validation.Locations;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using FAM.API.Validation.SubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Application.SubLocation.Command.CreateSubLocation;


namespace FAM.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateLocationCommand>, CreateLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateLocationCommand>, UpdateLocationCommandValidator>();
        services.AddScoped<IValidator<CreateSubLocationCommand>, CreateSubLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateSubLocationCommand>, UpdateSubLocationCommandValidator>();
    }  
    }
}