using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Users.Commands.UpdateFirstTimeUserPassword;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.Users
{
    public class PasswordChangeCommandValidator : AbstractValidator<FirstTimeUserPasswordCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public PasswordChangeCommandValidator(MaxLengthProvider maxLengthProvider)
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
                        RuleFor(x => x.Password)
                            .NotEmpty()
                            .WithMessage($"{nameof(FirstTimeUserPasswordCommand.Password)} {rule.Error}");
                        RuleFor(x => x.UserId)
                            .NotEmpty()
                            .WithMessage($"{nameof(FirstTimeUserPasswordCommand.UserId)} {rule.Error}");
                        break;

                    case "Password":
                     RuleFor(x => x.Password)
                     .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                     .WithMessage($"{nameof(FirstTimeUserPasswordCommand.Password)} {rule.Error}");
                     break;
                     case "NumericOnly":
                        RuleFor(x => x.UserId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(FirstTimeUserPasswordCommand.UserId)} {rule.Error}");
                        break;
                  
                    default:
                        
                        break;
                }
            }
        }
    }
}