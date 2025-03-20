using FluentValidation;
using Core.Domain.Entities;
using Core.Application.Users.Commands.UpdateUser;
using UserManagement.API.Validation.Common;
using Core.Application.Common.Interfaces.IUser;

namespace UserManagement.API.Validation.Users
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IUserQueryRepository _userQueryRepository;

        public UpdateUserCommandValidator(MaxLengthProvider maxLengthProvider,IUserQueryRepository userRepository)
        {
            var MaxLen = maxLengthProvider.GetMaxLength<User>("FirstName") ?? 25;

            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _userQueryRepository = userRepository;
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

                             RuleFor(x => x.UserId)
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
                            .EmailAddress()
                            .WithMessage($"{nameof(UpdateUserCommand.EmailId)} {rule.Error}");   
                        break; 

                    case "MobileNumber": 
                        RuleFor(x => x.Mobile) 
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(UpdateUserCommand.Mobile)} {rule.Error}"); 
                        break; 

                   
                    
                    case "AlreadyExists":
                           RuleFor(x =>  new { x.UserName, x.UserId })
                           .MustAsync(async (user, cancellation) => 
                        !await _userQueryRepository.AlreadyExistsAsync(user.UserName, user.UserId))             
                           .WithName("User Name")
                            .WithMessage($"{rule.Error}");
                            break;            
                    default:
                        // Handle unknown rule (log or throw)
                        // Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;
                }
            }
        }
    }
}