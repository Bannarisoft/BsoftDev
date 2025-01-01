using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Companies.Queries.GetCompanies;
using FluentValidation;

namespace BSOFT.API.Validation.Common.Companies
{
    public class CreateAddressValidator : AbstractValidator<CompanyAddressDTO>
    {
        private readonly List<ValidationRule> _validationRules;
        public CreateAddressValidator()
        {
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
                        // Apply NotEmpty validation
                        RuleFor(x => x.AddressLine1)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyAddressDTO.AddressLine1)} {rule.Error}");
                        RuleFor(x => x.PinCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyAddressDTO.AddressLine2)} {rule.Error}");
                            break;

                    case "Pincode":
                        RuleFor(x => x.PinCode)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CompanyAddressDTO.PinCode)} {rule.Error}");
                        break;

                    case "Telephone":
                        RuleFor(x => x.AlternatePhone)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CompanyAddressDTO.AlternatePhone)} {rule.Error}");
                        break;

                    case "NumericOnly":
                        RuleFor(x => x.CityId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CompanyAddressDTO.CityId)} {rule.Error}");
                        RuleFor(x => x.StateId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CompanyAddressDTO.StateId)} {rule.Error}");
                        RuleFor(x => x.CountryId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CompanyAddressDTO.CountryId)} {rule.Error}");
                        break;

                    default:                    
                        break;
                }
            }
        }
      
        
    }
}