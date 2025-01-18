using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;
using Core.Application.Companies.Commands.CreateCompany;
using Core.Domain.Entities;
using FluentValidation;

namespace BSOFT.API.Validation.Companies
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public CreateCompanyCommandValidator(MaxLengthProvider maxLengthProvider)
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
                        RuleFor(x => x.Company.File)
                            .NotNull()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.File)} {rule.Error}")
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.File)} {rule.Error}");

                             RuleFor(x => x.Company.CompanyName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyName)} {rule.Error}");

                        RuleFor(x => x.Company.LegalName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.LegalName)} {rule.Error}");

                            RuleFor(x => x.Company.GstNumber)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.GstNumber)} {rule.Error}");

                            RuleFor(x => x.Company.YearOfEstablishment)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.YearOfEstablishment)} {rule.Error}");

                            RuleFor(x => x.Company.Website)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.Website)} {rule.Error}");

                            RuleFor(x => x.Company.CompanyAddress.AddressLine1)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.AddressLine1)} {rule.Error}");

                        RuleFor(x => x.Company.CompanyAddress.PinCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.AddressLine2)} {rule.Error}");

                               RuleFor(x => x.Company.CompanyContact.Name)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Name)} {rule.Error}");
                        RuleFor(x => x.Company.CompanyContact.Designation)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Designation)} {rule.Error}");
                        RuleFor(x => x.Company.CompanyContact.Email)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Email)} {rule.Error}");
                        RuleFor(x => x.Company.CompanyContact.Phone)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Phone)} {rule.Error}");
                        break;

                    case "FileValidation":
                    RuleFor(x => x.Company.File)
                    .Must(file => IsValidFileType(file, rule.allowedExtensions))
                    .WithMessage($"{nameof(CreateCompanyCommand.Company.File)} {rule.Error}")
                    .Must(file => file.Length <= 2 * 1024 * 1024)
                    .WithMessage($"{nameof(CreateCompanyCommand.Company.File)} {rule.Error}");
                    break;

                     case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.Company.CompanyName)
                            .MaximumLength(companyNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyName)} {rule.Error} {companyNameMaxLength}");
                        RuleFor(x => x.Company.LegalName)
                            .MaximumLength(LegalNameMaxLength) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.LegalName)} {rule.Error} {LegalNameMaxLength}");
                        break;

                       case "GstFormat":
                        RuleFor(x => x.Company.GstNumber)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.GstNumber)} {rule.Error}");   
                        break;  
                        case "NumericOnly":
                        RuleFor(x => x.Company.YearOfEstablishment)
                            .InclusiveBetween(1900, DateTime.Now.Year)
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.YearOfEstablishment)} {rule.Error}");   

                              RuleFor(x => x.Company.CompanyAddress.CityId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.CityId)} {rule.Error}");

                        RuleFor(x => x.Company.CompanyAddress.StateId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.StateId)} {rule.Error}");

                        RuleFor(x => x.Company.CompanyAddress.CountryId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.CountryId)} {rule.Error}");
                        break;   

                        case "Website":      
                        RuleFor(x => x.Company.Website)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.Website)} {rule.Error}");   
                        break;  
                        case "MinLength":
                        RuleFor(x => x.Company.EntityId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateCompanyCommand.Company.EntityId)} {rule.Error} {0}");   
                        break;

                        case "Pincode":
                        RuleFor(x => x.Company.CompanyAddress.PinCode)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.PinCode)} {rule.Error}");
                        break;

                    case "Telephone":
                        RuleFor(x => x.Company.CompanyAddress.AlternatePhone)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyAddress.AlternatePhone)} {rule.Error}");

                         RuleFor(x => x.Company.CompanyContact.Phone)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Phone)} {rule.Error}");
                        break;

                        case "Email":
                        RuleFor(x => x.Company.CompanyContact.Email)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .WithMessage($"{nameof(CreateCompanyCommand.Company.CompanyContact.Email)} {rule.Error}");
                        break;
                   
                    default:
                        // Handle unknown rule (log or throw)
                        //Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
            
            
         
        }
             private bool IsValidFileType(IFormFile file, List<string> allowedExtensions)
                {
                    Console.WriteLine(file.FileName);
                    foreach (var extension in allowedExtensions)
                    {
                        Console.WriteLine(extension);
                        
                    }
                    
                    if (file == null) return false;
                
                    var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                    return allowedExtensions.Contains(fileExtension);
                }

    }
}