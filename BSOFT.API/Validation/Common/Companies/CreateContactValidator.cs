using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Companies.Queries.GetCompanies;
using FluentValidation;

namespace BSOFT.API.Validation.Common.Companies
{
    public class CreateContactValidator : AbstractValidator<CompanyContactDTO>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateContactValidator()
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
                        RuleFor(x => x.Name)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyContactDTO.Name)} {rule.Error}");
                        RuleFor(x => x.Designation)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyContactDTO.Designation)} {rule.Error}");
                        RuleFor(x => x.Email)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyContactDTO.Email)} {rule.Error}");
                        RuleFor(x => x.Phone)
                            .NotEmpty()
                            .WithMessage($"{nameof(CompanyContactDTO.Phone)} {rule.Error}");
                        break;

                    case "Email":
                        RuleFor(x => x.Email)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CompanyContactDTO.Email)} {rule.Error}");
                        break;

                    case "Telephone":
                        RuleFor(x => x.Phone)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CompanyContactDTO.Phone)} {rule.Error}");
                        break;
                }
                
            }
        }
    }
}