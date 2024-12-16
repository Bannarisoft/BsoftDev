using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using BSOFT.Application.Entity.Commands.CreateEntity;

namespace BSOFT.Application.Entity.Commands.CreateEntity
{
    public class CreateEntityCommandValidator : AbstractValidator<CreateEntityCommand>
    {
        public CreateEntityCommandValidator()
        {
        RuleFor(v => v.EntityName)
        .NotEmpty().WithMessage("Entity Name is required.")
        .MaximumLength(100).WithMessage("Entity Name is required and cannot exceed 100 characters..");

        RuleFor(v => v.EntityDescription)
            .NotEmpty().WithMessage("Entity Description is required.")
            .MaximumLength(250).WithMessage("Entity Description is required and cannot exceed 250 characters.");

        RuleFor(v => v.Address)
            .NotEmpty().WithMessage("Headquarters Address is required.")
            .MaximumLength(250).WithMessage("Headquarters Address is required and cannot exceed 200 characters.");

        RuleFor(v => v.Phone)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .MaximumLength(10).WithMessage("PhoneNumber must not exceed 10 characters.");


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