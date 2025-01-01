using FluentValidation;
using Core.Domain.Entities;
using Core.Application.Users.Commands.UpdateUser;

namespace BSOFT.API.Validation.Common.Users
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public UpdateUserCommandValidator(MaxLengthProvider maxLengthProvider)
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
                            .WithMessage($"{nameof(UpdateUserCommand.FirstName)} {rule.Error}");

                        RuleFor(x => x.LastName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateUserCommand.LastName)} {rule.Error}");

                        RuleFor(x => x.UserName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateUserCommand.UserName)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.FirstName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(UpdateUserCommand.FirstName)} {rule.Error} {MaxLen}");   

                             RuleFor(x => x.LastName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(UpdateUserCommand.LastName)} {rule.Error} {MaxLen}"); 

                             RuleFor(x => x.UserName)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(UpdateUserCommand.UserName)} {rule.Error} {MaxLen}"); 
                        break; 
                         
                    case "Email":
                        RuleFor(x => x.EmailId)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(UpdateUserCommand.EmailId)} {rule.Error}");   
                        break; 

                    case "MobileNumber": 
                        RuleFor(x => x.Mobile) 
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(UpdateUserCommand.Mobile)} {rule.Error}"); 
                        break; 

                    case "Password":
                        // Apply Password validation
                        RuleFor(x => x.PasswordHash)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(UpdateUserCommand.PasswordHash)} {rule.Error}");
                        break;  
                    
                    case "UserType":
                        RuleFor(x => x.UserType)
                            .InclusiveBetween(1, 2) // Assuming UserType should be between 1 and 2
                            .WithMessage($"{nameof(UpdateUserCommand.UserType)} {rule.Error}");
                        break;               
                    default:
                        // Handle unknown rule (log or throw)
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
        }
    }
}