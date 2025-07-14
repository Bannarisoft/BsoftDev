using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.CreateNotificationEventRule;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationEventRule
{
    public class CreateNotificationEventRuleCommandValidator : AbstractValidator<CreateNotificationEventRuleCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationEventRuleQuery _notificationEventRuleQuery;
        public CreateNotificationEventRuleCommandValidator(INotificationEventRuleQuery notificationEventRuleQuery)
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
                            .WithMessage($"{nameof(CreateNotificationEventRuleCommand.TemplateId)} {rule.Error}");

                            RuleFor(x => x.NotificationChannelId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationEventRuleCommand.NotificationChannelId)} {rule.Error}");

                            RuleFor(x => x.NotificationLevelHierarchyId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationEventRuleCommand.NotificationLevelHierarchyId)} {rule.Error}");

                            RuleFor(x => x.RecipientTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationEventRuleCommand.RecipientTypeId)} {rule.Error}");
                        break;
                    case "AlreadyExists":                       
                        RuleFor(x => new { x.TemplateId, x.NotificationChannelId, x.NotificationLevelHierarchyId, x.RecipientTypeId})
                          .MustAsync(async (Notification, cancellation) => !await _notificationEventRuleQuery.AlreadyExistsAsync(Notification.NotificationChannelId,Notification.TemplateId,Notification.NotificationLevelHierarchyId,Notification.RecipientTypeId))
                           .WithName("Notification Event Rule")
                             .WithMessage($"{rule.Error}");
                        break;

                }
            }
        }
    }
}