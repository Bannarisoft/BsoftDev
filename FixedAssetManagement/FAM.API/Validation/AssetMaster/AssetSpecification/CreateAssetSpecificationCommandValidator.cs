using Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification;
using Core.Domain.Entities.AssetMaster;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetSpecification
{
    public class CreateAssetSpecificationCommandValidator : AbstractValidator<CreateAssetSpecificationCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public CreateAssetSpecificationCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider            
            var assetSpecificationMaxLength = maxLengthProvider.GetMaxLength<AssetSpecifications>("SpecificationValue")??100;            

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
                        // Apply NotEmpty validation
                        RuleFor(x => x.SpecificationId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetSpecificationCommand.SpecificationId)} {rule.Error}");
                        RuleFor(x => x.SpecificationValue)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetSpecificationCommand.SpecificationValue)} {rule.Error}");                                        
                        break;
                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.SpecificationValue)
                            .MaximumLength(assetSpecificationMaxLength) 
                            .WithMessage($"{nameof(CreateAssetSpecificationCommand.SpecificationValue)} {rule.Error} {assetSpecificationMaxLength}");                                  
                        break;          
                    case "NumericOnly":       
                        RuleFor(x => x.AssetId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateAssetSpecificationCommand.AssetId)} {rule.Error}");    
                      /*   RuleFor(x => x.ManufactureId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateAssetSpecificationCommand.ManufactureId)} {rule.Error}");        */            
                        RuleFor(x => x.SpecificationId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateAssetSpecificationCommand.SpecificationId)} {rule.Error}");  
                        break;
                    default:                        
                        break;
                }
            }
        }
    }
}