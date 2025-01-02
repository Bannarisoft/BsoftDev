using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Core.Application.Companies.Commands;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Domain.Entities;
using BSOFT.API.Validation.Common;
using Core.Application.Companies.Queries.GetCompanies;

namespace BSOFT.API.Validation.Companies
{
    public class CreateCompanyValidator : AbstractValidator<CompanyDTO>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateCompanyValidator(MaxLengthProvider maxLengthProvider)
        {
            var companyNameMaxLength = maxLengthProvider.GetMaxLength<Company>("CompanyName") ?? 50;
            var LegalNameMaxLength = maxLengthProvider.GetMaxLength<Company>("LegalName") ?? 50;

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
                        RuleFor(x => x.CompanyName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyDTO.CompanyName)} {rule.Error}");
                        RuleFor(x => x.LegalName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyDTO.LegalName)} {rule.Error}");
                            RuleFor(x => x.GstNumber)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyDTO.GstNumber)} {rule.Error}");
                            RuleFor(x => x.YearOfEstablishment)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyDTO.YearOfEstablishment)} {rule.Error}");
                            RuleFor(x => x.Website)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyDTO.Website)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.CompanyName)
                            .MaximumLength(companyNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CompanyDTO.CompanyName)} {rule.Error} {companyNameMaxLength}");
                        RuleFor(x => x.LegalName)
                            .MaximumLength(LegalNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CompanyDTO.LegalName)} {rule.Error} {LegalNameMaxLength}");
                        break;
                         case "GstFormat":
                        RuleFor(x => x.GstNumber)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CompanyDTO.GstNumber)} {rule.Error}");   
                        break;  
                        case "NumericOnly":
                        RuleFor(x => x.YearOfEstablishment)
                            .InclusiveBetween(1900, DateTime.Now.Year)
                            .WithMessage($"{nameof(CompanyDTO.YearOfEstablishment)} {rule.Error}");   
                        break;   
                        case "Website":      
                        RuleFor(x => x.Website)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CompanyDTO.Website)} {rule.Error}");   
                        break;  
                        case "MinLength":
                        RuleFor(x => x.EntityId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CompanyDTO.EntityId)} {rule.Error} {0}");   
                        break;  
                    default:
                        // Handle unknown rule (log or throw)
                       // Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
            
        }
        
    }
}