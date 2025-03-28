using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Application.UserLogin.Commands.DeactivateUserSession;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.UserLogin
{
    public class DeactivateUserSessionCommandValidator : AbstractValidator<DeactivateUserSessionCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IUserQueryRepository _userQueryRepository;
        public DeactivateUserSessionCommandValidator(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
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
                        RuleFor(x => x.Username)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeactivateUserSessionCommand.Username)} {rule.Error}");
                        RuleFor(x => x.Password)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeactivateUserSessionCommand.Password)} {rule.Error}");
                     
                        break;
                        case "PasswordValidation":
                         RuleFor(x => x.Password)
                        .MustAsync(async (command, password, cancellation) =>
                            await PasswordValidation(command.Username, password))
                        .WithMessage($"{rule.Error}");
                        break;
                         default:
                       
                        break;
                }
                
            }
            
        }
         private async Task<bool> PasswordValidation(string userName, string newPassword)
           {
               var user = await _userQueryRepository.GetByUsernameAsync(userName);
               if (user != null)
               {
                   return BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash);
               }
               return false;
           }
    }
}