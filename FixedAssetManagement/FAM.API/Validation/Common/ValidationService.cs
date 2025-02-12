using FluentValidation;
using FAM.API.Validation.Locations;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using FAM.API.Validation.SubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Application.SubLocation.Command.CreateSubLocation;
using FAM.API.Validation.DepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;using Core.Application.AssetGroup.Command.CreateAssetGroup;
using FAM.API.Validation.AssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using FAM.API.Validation.AssetCategories;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetCategories.Command.UpdateAssetCategories;


namespace FAM.API.Validation.Common
{
    public class ValidationService
    {
    public void AddValidationServices(IServiceCollection services)
    {
        services.AddScoped<MaxLengthProvider>();
         services.AddScoped<IValidator<CreateAssetGroupCommand>, CreateAssetGroupCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetGroupCommand>, UpdateAssetGroupCommandValidator>();
        services.AddScoped<IValidator<CreateAssetCategoriesCommand>, CreateAssetCategoriesCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetCategoriesCommand >, UpdateAssetCategoriesCommandValidator>();
        services.AddScoped<IValidator<CreateLocationCommand>, CreateLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateLocationCommand>, UpdateLocationCommandValidator>();
        services.AddScoped<IValidator<CreateSubLocationCommand>, CreateSubLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateSubLocationCommand>, UpdateSubLocationCommandValidator>();
 services.AddScoped<IValidator<CreateDepreciationGroupCommand>, CreateDepreciationGroupCommandValidator>();
        services.AddScoped<IValidator<UpdateDepreciationGroupCommand>, UpdateDepreciationGroupCommandValidator>();
    }  
    }
}