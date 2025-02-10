using Core.Application.AssetGroup.Command.CreateAssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using FAM.API.Validation.AssetGroup;
using FluentValidation;

namespace FAM.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
        services.AddScoped<IValidator<CreateAssetGroupCommand>, CreateAssetGroupCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetGroupCommand>, UpdateAssetGroupCommandValidator>();
        
    }  
    }
}