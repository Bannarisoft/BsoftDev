using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationEventRule
{
    public class DeleteNotificationEventRuleCommandValidator : AbstractValidator<DeleteNotificationEventRuleCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationEventRuleQuery _notificationEventRuleQuery;
        public DeleteNotificationEventRuleCommandValidator(INotificationEventRuleQuery notificationEventRuleQuery)
        {
            _notificationEventRuleQuery = notificationEventRuleQuery;
            _validationRules = ValidationRuleLoader.LoadValidationRules();

            if (_validationRules == null || !_validationRules.Any())
            {
                throw new ArgumentException("Validation rules could not be loaded.");
            }
              foreach (var rule in _validationRules)
            {
                  switch (rule.Rule)
                {
                       case "NotEmpty":
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteNotificationEventRuleCommand.Id)} {rule.Error}");
                        break;
                        case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _notificationEventRuleQuery.NotFoundAsync(Id))             
                           .WithName("Notification Event Rule Id")
                            .WithMessage($"{rule.Error}");
                            break; 
                    default:
                        
                        break;
                }
            }
        }
    }
}