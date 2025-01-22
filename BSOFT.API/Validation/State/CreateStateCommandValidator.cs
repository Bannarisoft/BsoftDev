using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Core.Application.State.Commands;
using Core.Application.State.Commands.CreateState;
using Core.Domain.Entities;
using BSOFT.API.Validation.Common;

namespace BSOFT.API.Validation.State
{
    public class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateStateCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var stateCodeMaxLength = maxLengthProvider.GetMaxLength<States>("StateCode") ?? 5;
            var stateNameMaxLength = maxLengthProvider.GetMaxLength<States>("StateName") ?? 50;

            // Load validation rules from JSON or another source
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
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
                        RuleFor(x => x.StateName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateStateCommand.StateName)} {rule.Error}");
                        RuleFor(x => x.StateCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateStateCommand.StateCode)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.StateName)
                            .MaximumLength(stateNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateStateCommand.StateName)} {rule.Error} {stateNameMaxLength}");
                        RuleFor(x => x.StateCode)
                            .MaximumLength(stateCodeMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateStateCommand.StateCode)} {rule.Error} {stateCodeMaxLength}");
                        break;                 
                    default:
                        // Handle unknown rule (log or throw)
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
        }
    }
}