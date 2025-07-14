using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.CreateNotificationGroupMember;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationGroupMember
{
    public class CreateNotificationGroupMemberCommandValidator : AbstractValidator<CreateNotificationGroupMemberCommand>
    {
         private readonly List<Common.ValidationRule> _validationRules;
        private readonly INotificationGroupMemberQuery _notificationGroupQuery;
        public CreateNotificationGroupMemberCommandValidator(INotificationGroupMemberQuery notificationGroupQuery)
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
                            .WithMessage($"{nameof(CreateNotificationGroupMemberCommand.GroupId)} {rule.Error}");

                            RuleFor(x => x.UserId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationGroupMemberCommand.UserId)} {rule.Error}");
                        break;
                    case "AlreadyExists":                       
                        RuleFor(x => new { x.GroupId, x.UserId })
                          .MustAsync(async (Notification, cancellation) => !await _notificationGroupQuery.AlreadyExistsAsync(Notification.GroupId,Notification.UserId))
                           .WithName("Group Id")
                             .WithMessage($"{rule.Error}");
                        break;

                }
            }
        }
    }
}