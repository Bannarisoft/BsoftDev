using FluentValidation;
using FAM.API.Validation.Locations;
using Core.Application.Location.Command.CreateLocation;
using Core.Application.Location.Command.UpdateLocation;
using FAM.API.Validation.SubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Application.SubLocation.Command.CreateSubLocation;
using FAM.API.Validation.MiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using FAM.API.Validation.DepreciationGroup;using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using FAM.API.Validation.DepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.AssetGroup.Command.CreateAssetGroup;using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using FAM.API.Validation.AssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using FAM.API.Validation.AssetCategories;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetCategories.Command.UpdateAssetCategories;
using Core.Application.AssetSubCategories.Command.CreateAssetSubCategories;
using Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories;
using FAM.API.Validation.AssetSubCategories;using Core.Application.MiscMaster.Command.CreateMiscMaster;
using FAM.API.Validation.MiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.Manufacture.Commands.UpdateManufacture;
using FAM.API.Validation.Manufacture;
using Core.Application.Manufacture.Commands.CreateManufacture;
using FAM.API.Validation.AssetMaster.AssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;



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
		services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
         services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>(); 
		services.AddScoped<IValidator<CreateDepreciationGroupCommand>, CreateDepreciationGroupCommandValidator>();        services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>(); 
        services.AddScoped<IValidator<CreateDepreciationGroupCommand>, CreateDepreciationGroupCommandValidator>();
        services.AddScoped<IValidator<UpdateDepreciationGroupCommand>, UpdateDepreciationGroupCommandValidator>();
  		services.AddScoped<IValidator<CreateAssetSubCategoriesCommand>, CreateAssetSubCategoriesCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetSubCategoriesCommand >, UpdateAssetSubCategoriesCommandValidator>(); services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();        
services.AddScoped<IValidator<UpdateManufactureCommand>, UpdateManufactureCommandValidator>();
        services.AddScoped<IValidator<CreateManufactureCommand>, CreateManufactureCommandValidator>();
        services.AddScoped<IValidator<CreateAssetMasterGeneralCommand>, CreateAssetMasterGeneralCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetMasterGeneralCommand>, UpdateAssetMasterGeneralCommandValidator>();
    }  
    }
}