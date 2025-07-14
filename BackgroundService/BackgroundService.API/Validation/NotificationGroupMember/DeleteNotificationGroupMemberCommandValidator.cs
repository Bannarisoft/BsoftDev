using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.DeleteNotificationGroupMember;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationGroupMember
{
    public class DeleteNotificationGroupMemberCommandValidator : AbstractValidator<DeleteNotificationGroupMemberCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationGroupMemberQuery _notificationGroupQuery;
        public DeleteNotificationGroupMemberCommandValidator(INotificationGroupMemberQuery notificationGroupQuery)
        {
            _notificationGroupQuery = notificationGroupQuery;

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
                            .WithMessage($"{nameof(DeleteNotificationGroupMemberCommand.Id)} {rule.Error}");
                        break;
                        case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _notificationGroupQuery.NotFoundAsync(Id))             
                           .WithName("Notification Group Member Id")
                            .WithMessage($"{rule.Error}");
                            break; 
                    default:
                        
                        break;
                }
            }
        }
    }
}