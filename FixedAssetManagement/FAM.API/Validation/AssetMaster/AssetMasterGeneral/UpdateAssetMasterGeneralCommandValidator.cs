using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Domain.Entities;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetMasterGeneral
{
    public class UpdateAssetMasterGeneralCommandValidator : AbstractValidator<UpdateAssetMasterGeneralCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public UpdateAssetMasterGeneralCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var assetMasterGeneralCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetCode")??50;
            var assetMasterGeneralNameMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetName")??100;            
            var assetMasterGeneralDescriptionMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetDescription")??1000;  
            var assetMasterGeneralMachineCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("MachineCode")??100;  

            // Load validation rules from JSON or another source
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules is null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

            // Loop through the rules and apply them
            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":                        
                        RuleFor(x => x.AssetName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetName)} {rule.Error}");
                        RuleFor(x => x.AssetCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetCode)} {rule.Error}");
                        RuleFor(x => x.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.CompanyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.CompanyId)} {rule.Error}");
                        RuleFor(x => x.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.AssetCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetCategoryId)} {rule.Error}");
                        RuleFor(x => x.AssetSubCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetSubCategoryId)} {rule.Error}");   
                        RuleFor(x => x.AssetType)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetType)} {rule.Error}");   
                        RuleFor(x => x.Quantity)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.Quantity)} {rule.Error}");    
                        RuleFor(x => x.UOMId)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.UOMId)} {rule.Error}");    
                        RuleFor(x => x.AssetDescription)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetDescription)} {rule.Error}");    
                        RuleFor(x => x.WorkingStatus)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.WorkingStatus)} {rule.Error}");                                                    
                        break;
                    case "MaxLength":                        
                        RuleFor(x => x.AssetCode)
                            .MaximumLength(assetMasterGeneralCodeMaxLength) 
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetCode)} {rule.Error} {assetMasterGeneralCodeMaxLength}");
                        RuleFor(x => x.AssetName)
                            .MaximumLength(assetMasterGeneralNameMaxLength) 
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetName)} {rule.Error} {assetMasterGeneralNameMaxLength}");                             
                        RuleFor(x => x.AssetDescription)
                            .MaximumLength(assetMasterGeneralDescriptionMaxLength) 
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.AssetDescription)} {rule.Error} {assetMasterGeneralDescriptionMaxLength}");
                        RuleFor(x => x.AssetDescription)
                            .MaximumLength(assetMasterGeneralMachineCodeMaxLength) 
                            .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.MachineCode)} {rule.Error} {assetMasterGeneralMachineCodeMaxLength}");
                        break;          
                    case "NumericOnly":       
                        RuleFor(x => x.Quantity)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(UpdateAssetMasterGeneralCommand.Quantity)} {rule.Error}");                       
                        break;
                        
                    default:                        
                        break;
                }
            }
        }
    }
}