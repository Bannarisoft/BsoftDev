using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.ForgotUserPassword;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.Users
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotUserPasswordCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IUserQueryRepository _userQueryRepository;

        public ForgotPasswordCommandValidator(IUserQueryRepository userQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _userQueryRepository = userQueryRepository;

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
                            .WithMessage($"{nameof(ForgotUserPasswordCommand.UserName)} {rule.Error}");
                        break;

                    case "NotFound":
                         RuleFor(x => x.UserName)
                         .MustAsync(async (UserName, cancellation) => await _userQueryRepository.ValidateUsernameAsync(UserName))
                        .WithMessage($"{rule.Error}");
                        break;
                        case "InActive":
                         RuleFor(x => x.UserName)
                         .MustAsync(async (UserName, cancellation) => await _userQueryRepository.ValidateUserActiveAsync(UserName))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}