using FluentValidation;
using Core.Domain.Entities;
using Core.Application.Modules.Commands.UpdateModule;

namespace BSOFT.API.Validation.Common.Module
{
    public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
    {
        private readonly List<ValidationRule> _validationRules;
         public UpdateModuleCommandValidator(MaxLengthProvider maxLengthProvider)
         {
            var MaxLen = maxLengthProvider.GetMaxLength<Modules>("ModuleName") ?? 50;
             _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.ModuleName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateModuleCommand.ModuleName)} {rule.Error}");
                        break; 
                        case "MaxLength":
                        RuleFor(x => x.ModuleName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(UpdateModuleCommand.ModuleName)} {rule.Error} {MaxLen}");   
                        break;
                                  
                        default:
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
         }
    }
}