using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
        //     RuleFor(v => v.CompanyName)
        // .NotEmpty().WithMessage("CompanyName is required.")
        // .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

        //  RuleFor(v => v.LegalName)
        //     .NotEmpty().WithMessage("LegalName is required.")
        //     .MaximumLength(50).WithMessage("LegalName must not exceed 50 characters.");

        //  RuleFor(v => v.AddressLine1)
        //     .NotEmpty().WithMessage("Address1 is required.");

        // RuleFor(v => v.Phone)
        //     .NotEmpty().WithMessage("PhoneNumber is required.")
        //     .MaximumLength(10).WithMessage("PhoneNumber must not exceed 10 characters.");

        //     RuleFor(v => v.Email)
        //     .NotEmpty().WithMessage("Email is required.")
        //     .EmailAddress().WithMessage("EmailId must be in a valid format.")
        //     .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("EmailId must contain a valid domain.");

            // RuleFor(v => v.GstNumber)
            // .NotEmpty().WithMessage("GstNumber is required.")
            // .Matches(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$")
            // .WithMessage("Invalid GST number format.");

            // RuleFor(v => v.YearOfEstablishment)
            // .NotEmpty().WithMessage("YearOfEstablishment is required.");

            // RuleFor(v => v.Website)
            // .NotEmpty().WithMessage("Website is required.");

            // RuleFor(e => e.EntityId)
            // .GreaterThan(0).WithMessage("Entity must be greater than 0.")
            // .LessThanOrEqualTo(10000).WithMessage("Entity must not exceed 10000.");

            // int currentYear = DateTime.Now.Year;

            // RuleFor(e => e.YearOfEstablishment)
            // .InclusiveBetween(1800, currentYear)
            // .WithMessage($"YearOfEstablishment must be between 1800 and {currentYear}.")
            // .NotEmpty().WithMessage("YearOfEstablishment is required.");
        }
    }
}