using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;
using Core.Application.Companies.Commands.CreateCompany;
using FluentValidation;

namespace BSOFT.API.Validation.Companies
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
         private readonly List<ValidationRule> _validationRules;

        public CreateCompanyCommandValidator(MaxLengthProvider maxLengthProvider)
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
                        RuleFor(x => x.File)
                            .NotNull()
                            .WithMessage($"{nameof(CreateCompanyCommand.File)} {rule.Error}")
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateCompanyCommand.File)} {rule.Error}");
                        break;

                    case "FileValidation":
                    RuleFor(x => x.File)
                    .Must(file => IsValidFileType(file, rule.allowedExtensions))
                    .WithMessage($"{nameof(CreateCompanyCommand.File)} {rule.Error}")
                    .Must(file => file.Length <= 2 * 1024 * 1024)
                    .WithMessage($"{nameof(CreateCompanyCommand.File)} {rule.Error}");
                    break;

                   
                    default:
                        // Handle unknown rule (log or throw)
                        //Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
            RuleFor(C => C.Company).SetValidator(new CreateCompanyValidator(maxLengthProvider));
            RuleFor(C => C.CompanyAddresses).SetValidator(new CreateAddressValidator());
            RuleFor(C => C.CompanyContacts).SetValidator(new CreateContactValidator());
         
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