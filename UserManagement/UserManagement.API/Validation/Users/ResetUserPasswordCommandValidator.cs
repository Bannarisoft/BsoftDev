using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Users.Commands.ResetUserPassword;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.Users
{
    public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IChangePassword _ichangePassword;
        public ResetUserPasswordCommandValidator( IChangePassword ichangePassword)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _ichangePassword = ichangePassword;
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.UserName)
                            .NotEmpty()
                            .WithMessage($"{nameof(ResetUserPasswordCommand.UserName)} {rule.Error}");
                        RuleFor(x => x.Password)
                            .NotEmpty()
                            .WithMessage($"{nameof(ResetUserPasswordCommand.Password)} {rule.Error}");
                     
                        break;
                         case "PasswordHistory":
                         RuleFor(x => x.Password)
                      .MustAsync(async (command,Password, cancellation) => 
                    !await _ichangePassword.ValidatePasswordbyUserName(command.UserName, Password))
                        .WithMessage($"{rule.Error}");
                        break;

                         default:
                       
                        break;
                }
                
            }
        }
    }
}