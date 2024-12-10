using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(v => v.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(25).WithMessage("FirstName must not exceed 25 characters.");

            RuleFor(v => v.LastName)
                .MaximumLength(25).WithMessage("LastName must not exceed 25 characters.");   

            RuleFor(v => v.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(25).WithMessage("UserName must not exceed 25 characters.");

             RuleFor(v => v.UserType)
                .NotEmpty().WithMessage("UserType is required.")
                .Must(UserType => UserType == 1 || UserType == 2)
                .WithMessage("UserType must be either '1' for External or '2' for Internal.");
   

            RuleFor(v => v.EmailId)
                .NotEmpty().WithMessage("EmailId is required.")
                .EmailAddress().WithMessage("EmailId must be in a valid format.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("EmailId must contain a valid domain.")
                .Must(NotContainInvalidCharacters).WithMessage("EmailId contains invalid characters.");

            RuleFor(v => v.Mobile)
                .NotEmpty().WithMessage("Mobile is required.")
                .MaximumLength(10).WithMessage("Mobile must not exceed 10 characters.")
                .Matches(@"^\d+$").WithMessage("Mobile number must contain only numeric characters.");

            RuleFor(v => v.Role)
                .NotEmpty().WithMessage("Role is required.");
        }

    private bool NotContainInvalidCharacters(string email)
    {
        // Disallow spaces and certain special characters
        string invalidChars = "(),:;<>[]";
        return email.All(c => !invalidChars.Contains(c) && c != ' ');
    }
        
    }
}