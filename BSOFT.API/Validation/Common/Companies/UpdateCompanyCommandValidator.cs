using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Companies.Commands.UpdateCompany;
using FluentValidation;

namespace BSOFT.API.Validation.Common.Companies
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public UpdateCompanyCommandValidator(MaxLengthProvider maxLengthProvider)
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
                            .WithMessage($"{nameof(UpdateCompanyCommand.File)} {rule.Error}")
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateCompanyCommand.File)} {rule.Error}");
                        break;

                    case "FileValidation":
                    RuleFor(x => x.File)
                    .Must(file => IsValidFileType(file, rule.allowedExtensions))
                    .WithMessage($"{nameof(UpdateCompanyCommand.File)} {rule.Error}")
                    .Must(file => file.Length <= 2 * 1024 * 1024)
                    .WithMessage($"{nameof(UpdateCompanyCommand.File)} {rule.Error}");
                    break;

                    default:
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