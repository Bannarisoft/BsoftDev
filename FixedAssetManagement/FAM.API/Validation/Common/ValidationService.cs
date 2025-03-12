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
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails;
using FAM.API.Validation.AssetMaster.AssetPurchase;
using Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails;
using Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification;
using FAM.API.Validation.AssetMaster.AssetSpecification;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost;
using FAM.API.Validation.AssetMaster.AssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.UpdateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetWarranty.Commands.CreateAssetWarranty;
using FAM.API.Validation.AssetMaster.AssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.UpdateAssetWarranty;
using FAM.API.Validation.AssetMaster.AssetWaranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.UploadAssetWarranty;
using Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance;
using Core.Application.AssetMaster.AssetInsurance.Commands.UpdateAssetInsurance;
using FAM.API.Validation.AssetMaster.AssetInsurance;
using Core.Application.AssetMaster.AssetDisposal.Command.CreateAssetDisposal;
using FAM.API.Validation.AssetMaster.AssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Command.UpdateAssetDisposal;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Commands.UpdateAssetTranferIssueApproval;
using Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue;
using FAM.API.Validation.AssetMaster.AssetTransferIssueApproval;
using FAM.API.Validation.AssetMaster.AssetTransferIssue;
using Core.Application.AssetMaster.AssetTransferIssue.Command.UpdateAssetTransferIssue;


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
        services.AddScoped<IValidator<UpdateAssetLocationCommand>, UpdateAssetLocationCommandValidator>();		
        services.AddScoped<IValidator<CreateAssetPurchaseDetailCommand>, CreateAssetPurchaseDetailCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetPurchaseDetailCommand>, UpdateAssetPurchaseDetailCommandValidator>();        
		services.AddScoped<IValidator<CreateAssetSpecificationCommand>, CreateAssetSpecificationCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetSpecificationCommand>, UpdateAssetSpecificationCommandValidator>();
		services.AddScoped<IValidator<CreateAssetAdditionalCostCommand>, CreateAssetAdditionalCostCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetAdditionalCostCommand>, UpdateAssetAdditionalCostCommandValidator>();
        services.AddScoped<IValidator<CreateAssetWarrantyCommand>, CreateAssetWarrantyCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetWarrantyCommand>, UpdateAssetWarrantyCommandValidator>();
        services.AddScoped<IValidator<UploadFileAssetWarrantyCommand>, UploadAssetWarrantyCommandValidator>();
        services.AddScoped<IValidator<CreateAssetInsuranceCommand>, CreateAssetInsuranceCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetInsuranceCommand>, UpdateAssetInsuranceCommandValidator>();
        services.AddScoped<IValidator<CreateAssetDisposalCommand>, CreateAssetDisposalCommandValidator>();

        services.AddScoped<IValidator<UpdateAssetDisposalCommand>, UpdateAssetDisposalCommandValidator>();  
        services.AddScoped<IValidator<UpdateAssetTranferIssueApprovalCommand>, UpdateAssetTransferIssueApprovalValidator>();                
        services.AddScoped<IValidator<CreateAssetTransferIssueCommand>, CreateAssetTransferIssueCommandValidator>();
        services.AddScoped<IValidator<UpdateAssetTransferIssueCommand>, UpdateAssetTransferIssueCommandValidator>();

    }  
    }
}