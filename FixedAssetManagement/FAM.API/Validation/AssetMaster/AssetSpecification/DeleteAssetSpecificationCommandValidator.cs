using Core.Application.AssetMaster.AssetSpecification.Commands.DeleteAssetSpecification;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.AssetMaster.AssetSpecification
{
    public class DeleteAssetSpecificationCommandValidator  : AbstractValidator<DeleteAssetSpecificationCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IAssetSpecificationQueryRepository _assetQueryRepository;
        public DeleteAssetSpecificationCommandValidator( IAssetSpecificationQueryRepository assetQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _assetQueryRepository = assetQueryRepository;

            
             if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

             foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteAssetSpecificationCommand.Id)} {rule.Error}");
                        break;
               
                    default:                        
                        break;
                }
            }
            
        }
    }
}