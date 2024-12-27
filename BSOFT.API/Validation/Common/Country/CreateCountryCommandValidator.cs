using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Core.Application.Country.Commands;
using Core.Application.Country.Commands.CreateCountry;
using Core.Domain.Entities;
using BSOFT.API.Validation.Common;

namespace BSOFT.API.Validation.Common.Country
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateCountryCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            // Get max lengths dynamically using MaxLengthProvider
            var countryCodeMaxLength = maxLengthProvider.GetMaxLength<Countries>("CountryCode") ?? 6;
            var countryNameMaxLength = maxLengthProvider.GetMaxLength<Countries>("CountryName") ?? 50;

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
                        RuleFor(x => x.CountryName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCountryCommand.CountryName)} {rule.Error}");
                        RuleFor(x => x.CountryCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCountryCommand.CountryCode)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.CountryName)
                            .MaximumLength(countryNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCountryCommand.CountryName)} {rule.Error} {countryNameMaxLength}");
                        RuleFor(x => x.CountryCode)
                            .MaximumLength(countryCodeMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCountryCommand.CountryCode)} {rule.Error} {countryCodeMaxLength}");
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