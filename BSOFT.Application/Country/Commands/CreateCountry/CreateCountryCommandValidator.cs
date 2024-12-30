using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Newtonsoft.Json;
using BSOFT.Application.Validation;
using System.Security.Cryptography.X509Certificates;
using BSOFT.Application.Country.Commands;

namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public CreateCountryCommandValidator()
        {
            //var validationRules = LoadValidationRules();
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules  == null || !_validationRules .Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
            int maxLength ;
             foreach (var rule in _validationRules )
            {
                //string propertyName = rule.Property;
                //string errorMessage = $"{propertyName} {rule.Error}"; 
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.CountryName)
                            .NotEmpty()
                            .WithMessage($"CountryName {rule.Error}");   
                         RuleFor(x => x.CountryCode)
                            .NotEmpty()
                            .WithMessage($"Code  {rule.Error}");                                  
                        break;
                    case "MaxLength":
                        maxLength=50;
                        RuleFor(x =>x.CountryName)
                            .MaximumLength(maxLength) // or some other length from rule.Length                
                            .WithMessage($"{nameof(CreateUnitCommand.UnitName)} {rule.Error} {maxLength}");
                         maxLength=6;
                         RuleFor(x => x.CountryCode)
                            .MaximumLength(maxLength) // or some other length from rule.Length                
                            .WithMessage($"{nameof(CreateUnitCommand.ShortName)} {rule.Error} {maxLength}");                          
                        break;                    
                   default:
                    // Log a warning or handle unknown rule
                    Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                    break;
                }      
            }
        }
}
}