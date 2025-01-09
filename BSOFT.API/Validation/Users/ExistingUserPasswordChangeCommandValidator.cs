using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;
using Core.Application.Users.Commands.ChangeUserPassword;
using FluentValidation;

namespace BSOFT.API.Validation.Users
{
    public class ExistingUserPasswordChangeCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public ExistingUserPasswordChangeCommandValidator()
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
                        RuleFor(x => x.OldPassword)
                            .NotEmpty()
                            .WithMessage($"{nameof(ChangeUserPasswordCommand.OldPassword)} {rule.Error}");
                        RuleFor(x => x.NewPassword)
                            .NotEmpty()
                            .WithMessage($"{nameof(ChangeUserPasswordCommand.NewPassword)} {rule.Error}");
                        RuleFor(x => x.UserId)
                            .NotEmpty()
                            .WithMessage($"{nameof(ChangeUserPasswordCommand.UserId)} {rule.Error}");
                     
                        break;

                     case "NumericOnly":
                        RuleFor(x => x.UserId)
                        .InclusiveBetween(1, int.MaxValue)
                        .WithMessage($"{nameof(ChangeUserPasswordCommand.UserId)} {rule.Error}");
                        break;

                    case "Password":
                     RuleFor(x => x.NewPassword)
                     .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                     .WithMessage($"{nameof(ChangeUserPasswordCommand.NewPassword)} {rule.Error}");
                     RuleFor(x => x.OldPassword)
                     .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                     .WithMessage($"{nameof(ChangeUserPasswordCommand.OldPassword)} {rule.Error}");
                     break;

                      default:
                        
                        break;
                }
            }
        }
    }
}