using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PwdComplexityRule.Commands.CreatePasswordComplexityRule;
using UserManagement.API.Validation.Common;
using Core.Domain.Entities;
using Serilog;

namespace UserManagement.API.Validation.PasswordComplexityrule
{
    public class CreatePasswordComplexityRuleCommandValidator   : AbstractValidator<CreatePasswordComplexityRuleCommand>
    { 

          private readonly List<ValidationRule> _validationRules;
        public CreatePasswordComplexityRuleCommandValidator(MaxLengthProvider maxLengthProvider)
        {
                var PwdComplexityRuleMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.PasswordComplexityRule>("PwdComplexityRule") ?? 150;
            
            _validationRules = ValidationRuleLoader.LoadValidationRules();

            if (_validationRules == null || !_validationRules.Any())
            {
                throw new ArgumentException("Validation rules could not be loaded.");
            }
          foreach   (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.PwdComplexityRule)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreatePasswordComplexityRuleCommand.PwdComplexityRule)} {rule.Error}");
                        break;
                    case "MaxLength":
                        RuleFor(x => x.PwdComplexityRule)
                            .MaximumLength(maxLengthProvider.GetMaxLength<PasswordComplexityRule>("PwdComplexityRule") ?? 50)
                            .WithMessage($"{nameof(CreatePasswordComplexityRuleCommand.PwdComplexityRule)} {rule.Error} {maxLengthProvider.GetMaxLength<PasswordComplexityRule>("PwdComplexityRule") ?? 150}");
                        break;
                    default:
                        Log.Information($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }

        }
        


    }
}