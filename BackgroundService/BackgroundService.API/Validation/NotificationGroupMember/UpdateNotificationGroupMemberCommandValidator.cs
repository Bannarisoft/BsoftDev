using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.UpdateNotificationGroupMember;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationGroupMember
{
    public class UpdateNotificationGroupMemberCommandValidator : AbstractValidator<UpdateNotificationGroupMemberCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationGroupMemberQuery _notificationGroupQuery;

        public UpdateNotificationGroupMemberCommandValidator(INotificationGroupMemberQuery notificationGroupQuery)
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
                        RuleFor(x => x.GroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationGroupMemberCommand.GroupId)} {rule.Error}");

                            RuleFor(x => x.UserId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationGroupMemberCommand.UserId)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                        RuleFor(x => new { x.GroupId,x.UserId, x.Id })
                         .MustAsync(async (notification, cancellation) =>
                      !await _notificationGroupQuery.AlreadyExistsAsync(notification.GroupId,notification.UserId, notification.Id))
                         .WithName("Group Name")
                          .WithMessage($"{rule.Error}");
                        break;
                        
                    case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _notificationGroupQuery.NotFoundAsync(Id))             
                           .WithName("Notification Group Member Id")
                            .WithMessage($"{rule.Error}");
                            break; 

                }
            }
        }
    }
}