using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationEventRule
{
    public class UpdateNotificationEventRuleCommandValidator : AbstractValidator<UpdateNotificationEventRuleCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationEventRuleQuery _notificationEventRuleQuery;
        public UpdateNotificationEventRuleCommandValidator(INotificationEventRuleQuery notificationEventRuleQuery)
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
                         RuleFor(x => x.TemplateId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationEventRuleCommand.TemplateId)} {rule.Error}");

                            RuleFor(x => x.NotificationChannelId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationEventRuleCommand.NotificationChannelId)} {rule.Error}");

                            RuleFor(x => x.NotificationLevelHierarchyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationEventRuleCommand.NotificationLevelHierarchyId)} {rule.Error}");

                            RuleFor(x => x.RecipientTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationEventRuleCommand.RecipientTypeId)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                        RuleFor(x => new { x.TemplateId, x.NotificationChannelId, x.NotificationLevelHierarchyId, x.RecipientTypeId,x.Id })
                         .MustAsync(async (Notification, cancellation) =>
                      !await _notificationEventRuleQuery.AlreadyExistsAsync(Notification.NotificationChannelId,Notification.TemplateId,Notification.NotificationLevelHierarchyId,Notification.RecipientTypeId, Notification.Id))
                         .WithName("Notification Event Rule")
                          .WithMessage($"{rule.Error}");
                        break;
                        
                    case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _notificationEventRuleQuery.NotFoundAsync(Id))             
                           .WithName("Notification Event Rule Id")
                            .WithMessage($"{rule.Error}");
                            break; 

                }
            }
        }
    }
}