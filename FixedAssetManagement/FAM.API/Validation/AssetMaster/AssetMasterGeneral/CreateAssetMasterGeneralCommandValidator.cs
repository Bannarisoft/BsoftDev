

using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Domain.Entities;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommandValidator : AbstractValidator<CreateAssetMasterGeneralCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateAssetMasterGeneralCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var assetMasterGeneralCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetCode")??50;
            var assetMasterGeneralNameMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetName")??100;            
            var assetMasterGeneralDescriptionMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("AssetDescription")??250;  
            var assetMasterGeneralMachineCodeMaxLength = maxLengthProvider.GetMaxLength<AssetMasterGenerals>("MachineCode")??50;  

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
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetName)} {rule.Error}");
                        RuleFor(x => x.AssetCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetCode)} {rule.Error}");
                        RuleFor(x => x.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.CompanyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.CompanyId)} {rule.Error}");
                        RuleFor(x => x.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetGroupId)} {rule.Error}");
                        RuleFor(x => x.AssetCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetCategoryId)} {rule.Error}");
                        RuleFor(x => x.AssetSubCategoryId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetSubCategoryId)} {rule.Error}");   
                        RuleFor(x => x.AssetType)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetType)} {rule.Error}");   
                        RuleFor(x => x.Quantity)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.Quantity)} {rule.Error}");    
                        RuleFor(x => x.UOMId)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.UOMId)} {rule.Error}");    
                        RuleFor(x => x.AssetDescription)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetDescription)} {rule.Error}");    
                        RuleFor(x => x.WorkingStatus)
                            .NotEmpty()                            
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.WorkingStatus)} {rule.Error}");                                                    
                        break;
                    case "MaxLength":                        
                        RuleFor(x => x.AssetCode)
                            .MaximumLength(assetMasterGeneralCodeMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetCode)} {rule.Error} {assetMasterGeneralCodeMaxLength}");
                        RuleFor(x => x.AssetName)
                            .MaximumLength(assetMasterGeneralNameMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetName)} {rule.Error} {assetMasterGeneralNameMaxLength}");                             
                        RuleFor(x => x.AssetDescription)
                            .MaximumLength(assetMasterGeneralDescriptionMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.AssetDescription)} {rule.Error} {assetMasterGeneralDescriptionMaxLength}");
                        RuleFor(x => x.AssetDescription)
                            .MaximumLength(assetMasterGeneralMachineCodeMaxLength) 
                            .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.MachineCode)} {rule.Error} {assetMasterGeneralMachineCodeMaxLength}");
                        break;          
                    case "NumericOnly":       
                        RuleFor(x => x.Quantity)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateAssetMasterGeneralCommand.Quantity)} {rule.Error}");                       
                        break;
                        
                    default:                        
                        break;
                }
            }
        }
    }
}