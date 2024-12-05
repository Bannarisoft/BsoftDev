using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace BSOFT.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(v => v.CompanyName)
        .NotEmpty().WithMessage("CompanyName is required.")
        .MaximumLength(50).WithMessage("Name must not exceed 200 characters.");

         RuleFor(v => v.LegalName)
            .NotEmpty().WithMessage("LegalName is required.");

         RuleFor(v => v.Address1)
            .NotEmpty().WithMessage("Address1 is required.");

        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.");
        }
    }
}