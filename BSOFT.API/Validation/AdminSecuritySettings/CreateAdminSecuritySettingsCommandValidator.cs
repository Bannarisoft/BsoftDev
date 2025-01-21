using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using BSOFT.API.Validation.Common;
using Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings;


namespace BSOFT.API.Validation.AdminSecuritySettings
{
    public class CreateAdminSecuritySettingsCommandValidator : AbstractValidator<CreateAdminSecuritySettingsCommand>
    {
        
         private readonly List<ValidationRule> _validationRules;

      public CreateAdminSecuritySettingsCommandValidator(MaxLengthProvider maxLengthProvider)
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
                            // Apply NotEmpty validation
                            RuleFor(x => x.PasswordHistoryCount)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreateAdminSecuritySettingsCommand.PasswordHistoryCount)} {rule.Error}");
                            RuleFor(x => x.SessionTimeoutMinutes)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreateAdminSecuritySettingsCommand.SessionTimeoutMinutes)} {rule.Error}");
                                
                            break;  
                }
                
           }
           }


    }
}