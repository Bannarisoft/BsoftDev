using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;
using Core.Application.UserLogin.Commands.UserLogin;
using Core.Domain.Entities;
using FluentValidation;

namespace BSOFT.API.Validation.UserLogin
{
    public class UserLoginCommandValidator: AbstractValidator<UserLoginCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public UserLoginCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var MaxLen = maxLengthProvider.GetMaxLength<User>("UserName") ?? 25;
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
                            .WithMessage($"{nameof(UserLoginCommand.Username)} {rule.Error}");
                        break;

                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.Username)
                            .MaximumLength(MaxLen) // Dynamic value from MaxLengthProvider
                            .WithMessage($"{nameof(UserLoginCommand.Username)} {rule.Error} {MaxLen}");   
                        break; 

                    case "Password":
                        RuleFor(x => x.Password)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                        .WithMessage($"{nameof(UserLoginCommand.Password)} {rule.Error}");
                        break; 
                    default:                        
                        break;
                }
            }
        }
    }
}