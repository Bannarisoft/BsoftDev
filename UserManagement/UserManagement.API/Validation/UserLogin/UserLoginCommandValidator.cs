using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Validation.Common;
using Core.Application.UserLogin.Commands.UserLogin;
using Core.Domain.Entities;
using FluentValidation;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Application.Common.Interfaces.ICompanySettings;

namespace UserManagement.API.Validation.UserLogin
{
    public class UserLoginCommandValidator: AbstractValidator<UserLoginCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly ICompanyQuerySettings _companyQuerySettings;
        private readonly IUserSessionRepository _userSessionRepository;
        public UserLoginCommandValidator(MaxLengthProvider maxLengthProvider, IUserQueryRepository userQueryRepository, ICompanyQuerySettings companyQuerySettings,IUserSessionRepository userSessionRepository)
        {
            var MaxLen = maxLengthProvider.GetMaxLength<User>("UserName") ?? 25;
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _companyQuerySettings = companyQuerySettings;
            _userQueryRepository = userQueryRepository;
            _userSessionRepository = userSessionRepository;
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
                            .WithMessage($"{nameof(UserLoginCommand.Username)} {rule.Error}");
                         RuleFor(x => x.Password)
                            .NotEmpty()
                            .WithMessage($"{nameof(UserLoginCommand.Password)} {rule.Error}");
                        break;

                    case "MaxLength":
                        RuleFor(x => x.Username)
                            .MaximumLength(MaxLen) 
                            .WithMessage($"{nameof(UserLoginCommand.Username)} {rule.Error} {MaxLen}");   
                        break; 
 
                    case "NotFound":
                           RuleFor(x => x.Username )
                           .MustAsync(async (Username, cancellation) => 
                        await _userQueryRepository.AlreadyExistsAsync(Username))             
                           .WithName("User Name")
                            .WithMessage($"{rule.Error}");

                            RuleFor(x => x.Username )
                           .MustAsync(async (Username, cancellation) => 
                        await _companyQuerySettings.BeforeLoginNotFoundValidation(Username))             
                           .WithName("User Name")
                            .WithMessage("User Admin Settings not found");
                            break;
                    case "UserSession":
                           RuleFor(x => x.Username )
                           .MustAsync(async (Username, cancellation) => 
                        !await _userSessionRepository.ValidateUserSession(Username))
                            .WithMessage($"{rule.Error}");
                            break;  
                    case "UserRole":
                           RuleFor(x => x.Username )
                           .MustAsync(async (Username, cancellation) => 
                        await _userQueryRepository.ValidateUserRolesAsync(Username))
                            .WithMessage($"{rule.Error}");
                            break; 
                            
                    default:                        
                        break;
                }
            }
        }
    }
}