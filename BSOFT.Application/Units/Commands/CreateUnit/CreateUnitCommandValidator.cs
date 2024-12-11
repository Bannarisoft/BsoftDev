using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using BSOFT.Application.Units.Commands.CreateUnit;

namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
    {
        public CreateUnitCommandValidator()
        {
             RuleFor(v => v.Name)
        .NotEmpty().WithMessage("UnitName is required.")
        .MaximumLength(50).WithMessage("UnitName must not exceed 50 characters.");

         RuleFor(v => v.ShortName)
            .NotEmpty().WithMessage("ShortName is required.")
            .MaximumLength(6).WithMessage("ShortName must not exceed 6 characters.");

         RuleFor(v => v.Address1)
            .NotEmpty().WithMessage("Address1 is required.");

        RuleFor(v => v.CoId)
            .NotEmpty().WithMessage("Company is required.");

         RuleFor(v => v.DivId)
            .NotEmpty().WithMessage("Division is required.");
         
        RuleFor(v => v.UnitHeadName)
            .NotEmpty().WithMessage("UnitHeadName is required.")
            .MaximumLength(20).WithMessage("UnitHeadName must not exceed 20 characters.");


        RuleFor(v => v.Mobile)
            .NotEmpty().WithMessage("MobileNo is required.")
            .MaximumLength(10).WithMessage("MobileNo must not exceed 10 characters.");

            RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("EmailId must be in a valid format.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("EmailId must contain a valid domain.")
            .Must(NotContainInvalidCharacters).WithMessage("EmailId contains invalid characters.");         
        }
        
    private bool NotContainInvalidCharacters(string email)
    {
        // Disallow spaces and certain special characters
        string invalidChars = "(),:;<>[]";
        return email.All(c => !invalidChars.Contains(c) && c != ' ');
    }
    }
}