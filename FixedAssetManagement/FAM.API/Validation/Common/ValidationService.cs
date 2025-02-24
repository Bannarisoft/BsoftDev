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
using FAM.API.Validation.DepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.AssetGroup.Command.CreateAssetGroup;
using FAM.API.Validation.AssetGroup;
using Core.Application.AssetGroup.Command.UpdateAssetGroup;
using FAM.API.Validation.AssetCategories;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using Core.Application.AssetCategories.Command.UpdateAssetCategories;
using Core.Application.AssetSubCategories.Command.CreateAssetSubCategories;
using Core.Application.AssetSubCategories.Command.UpdateAssetSubCategories;
using FAM.API.Validation.AssetSubCategories;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using FAM.API.Validation.MiscMaster;
using Core.Application.MiscMaster.Command.UpdateMiscMaster;
using Core.Application.Manufacture.Commands.UpdateManufacture;
using FAM.API.Validation.Manufacture;
using Core.Application.Manufacture.Commands.CreateManufacture;
using FAM.API.Validation.AssetMaster.AssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral;
using Core.Application.UOM.Command.CreateUOM;
using Core.Application.UOM.Command.UpdateUOM;
using FAM.API.Validation.UOM;
using Core.Application.SpecificationMaster.Commands.UpdateSpecificationMaster;
using FAM.API.Validation.SpecificationMaster;
using Core.Application.SpecificationMaster.Commands.CreateSpecificationMaster;
using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using FAM.API.Validation.AssetMaster.AssetLocation;
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using FAM.API.Validation.AssetMaster.AssetPurchase;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;using Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification;
using FAM.API.Validation.AssetMaster.AssetSpecification;


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
		services.AddScoped<IValidator<CreateDepreciationGroupCommand>, CreateDepreciationGroupCommandValidator>();        
        services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>(); 
        services.AddScoped<IValidator<CreateDepreciationGroupCommand>, CreateDepreciationGroupCommandValidator>();
        services.AddScoped<IValidator<UpdateDepreciationGroupCommand>, UpdateDepreciationGroupCommandValidator>();
  		services.AddScoped<IValidator<CreateAssetSubCategoriesCommand>, CreateAssetSubCategoriesCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetSubCategoriesCommand >, UpdateAssetSubCategoriesCommandValidator>(); 
        services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
        services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();        
		services.AddScoped<IValidator<UpdateManufactureCommand>, UpdateManufactureCommandValidator>();
        services.AddScoped<IValidator<UpdateManufactureCommand>, UpdateManufactureCommandValidator>();
        services.AddScoped<IValidator<CreateManufactureCommand>, CreateManufactureCommandValidator>();
		services.AddScoped<IValidator<CreateAssetMasterGeneralCommand>, CreateAssetMasterGeneralCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetMasterGeneralCommand>, UpdateAssetMasterGeneralCommandValidator>();
        services.AddScoped<IValidator<UploadFileAssetMasterGeneralCommand>, UploadAssetMasterGeneralCommandValidator>();
        services.AddScoped<IValidator<CreateUOMCommand>, CreateUOMCommandValidator>();
        services.AddScoped<IValidator<UpdateUOMCommand>, UpdateUOMCommandValidator>();
        services.AddScoped<IValidator<UpdateSpecificationMasterCommand>, UpdateSpecificationMasterCommandValidator>();
        services.AddScoped<IValidator<CreateSpecificationMasterCommand>, CreateSpecificationMasterCommandValidator>();
		services.AddScoped<IValidator<CreateAssetLocationCommand>, CreateAssetLocationCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetLocationCommand>, UpdateAssetLocationCommandValidator>();		services.AddScoped<IValidator<CreateAssetPurchaseDetailCommand>, CreateAssetPurchaseDetailCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetPurchaseDetailCommand>, UpdateAssetPurchaseDetailCommandValidator>();        
		services.AddScoped<IValidator<CreateAssetSpecificationCommand>, CreateAssetSpecificationCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetSpecificationCommand>, UpdateAssetSpecificationCommandValidator>();
    }  
    }
}