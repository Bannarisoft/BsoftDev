using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Commands.DeleteSpecificationMaster;
using FAM.API.Validation.Common;
using FluentValidation;

namespace FAM.API.Validation.SpecificationMaster
{
    public class DeleteSpecificationMasterCommandValidator : AbstractValidator<DeleteSpecificationMasterCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly ISpecificationMasterQueryRepository _assetQueryRepository;
        public DeleteSpecificationMasterCommandValidator( ISpecificationMasterQueryRepository assetQueryRepository)
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
                            .WithMessage($"{nameof(DeleteSpecificationMasterCommand.Id)} {rule.Error}");
                        break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _assetQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:                        
                        break;
                }
            }
            
        }
    }
}