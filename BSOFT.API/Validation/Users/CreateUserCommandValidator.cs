using FluentValidation;
using Core.Domain.Entities;
using Core.Application.Users.Commands.CreateUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;

namespace BSOFT.API.Validation.Users
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateUserCommandValidator(MaxLengthProvider maxLengthProvider)
        {
           var MaxLen = maxLengthProvider.GetMaxLength<User>("FirstName") ?? 25;

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
                        RuleFor(x => x.FirstName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUserCommand.FirstName)} {rule.Error}");

                        RuleFor(x => x.LastName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUserCommand.LastName)} {rule.Error}");

                        RuleFor(x => x.UserName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateUserCommand.UserName)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.FirstName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUserCommand.FirstName)} {rule.Error} {MaxLen}");   

                             RuleFor(x => x.LastName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUserCommand.LastName)} {rule.Error} {MaxLen}"); 

                             RuleFor(x => x.UserName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(CreateUserCommand.UserName)} {rule.Error} {MaxLen}"); 
                        break; 
                         
                    case "Email":
                        RuleFor(x => x.EmailId)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateUserCommand.EmailId)} {rule.Error}");   
                        break; 

                    case "MobileNumber": 
                        RuleFor(x => x.Mobile) 
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(CreateUserCommand.Mobile)} {rule.Error}"); 
                        break; 

                    case "Password":
                        RuleFor(x => x.Password)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(CreateUserCommand.Password)} {rule.Error}");
                        break; 
                        
                    case "UserType":
                        RuleFor(x => x.UserType)
                            .InclusiveBetween(1, 2) // Assuming UserType should be between 1 and 2
                            .WithMessage($"{nameof(CreateUserCommand.UserType)} {rule.Error}");
                        break;

                    default:                        
                        break;
                }
            }
        }
    }
}