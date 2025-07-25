using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

using Core.Domain.Entities;
using UserManagement.API.Validation.Common;
using Core.Application.City.Commands.CreateCity;

namespace UserManagement.API.Validation.City
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateCityCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var cityCodeMaxLength = maxLengthProvider.GetMaxLength<Cities>("CityCode") ?? 5;
            var cityNameMaxLength = maxLengthProvider.GetMaxLength<Cities>("CityName") ?? 50;

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
                        RuleFor(x => x.CityName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCityCommand.CityName)} {rule.Error}");
                        RuleFor(x => x.CityCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCityCommand.CityCode)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.CityName)
                            .MaximumLength(cityNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCityCommand.CityName)} {rule.Error} {cityNameMaxLength}");
                        RuleFor(x => x.CityCode)
                            .MaximumLength(cityNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCityCommand.CityCode)} {rule.Error} {cityCodeMaxLength}");
                        break;                 
                    default:                        
                        break;
                }
            }
        }
    }
}