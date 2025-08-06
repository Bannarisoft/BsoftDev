using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationLevelHierarchy
{
    public class UpdateNotificationLevelHierarchyCommandValidator : AbstractValidator<UpdateNotificationLevelHierarchyCommand>
    {        
        private readonly List<ValidationRule> _validationRules;                    
        private readonly INotificationLevelHierarchyCommandRepository _NotificationLevelHierarchyCommandRepository;
        private readonly INotificationLevelHierarchyQueryRepository _NotificationLevelHierarchyQueryRepository;  
        public UpdateNotificationLevelHierarchyCommandValidator(MaxLengthProvider maxLengthProvider, INotificationLevelHierarchyCommandRepository NotificationLevelHierarchyCommandRepository,INotificationLevelHierarchyQueryRepository NotificationLevelHierarchyQueryRepository)
        {
            _NotificationLevelHierarchyQueryRepository = NotificationLevelHierarchyQueryRepository;            
            _NotificationLevelHierarchyCommandRepository = NotificationLevelHierarchyCommandRepository;
            var maxLength = maxLengthProvider.GetMaxLength<Domain.Entities.Notification.NotificationLevelHierarchy>("Description") ?? 250;

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
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.Description)} {rule.Error}");
                        RuleFor(x => x.NotificationConfigId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.NotificationConfigId)} {rule.Error}");
                        RuleFor(x => x.TargetTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.TargetTypeId)} {rule.Error}");
                        RuleFor(x => x.TargetId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.TargetId)} {rule.Error}");
                        RuleFor(x => x.ApprovalModeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.ApprovalModeId)} {rule.Error}");    
                        break;
                    case "MaxLength":
                        RuleFor(x => x.Description)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.Description)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                         RuleFor(x => x.NotificationConfigId)
                        .NotEmpty()
                        .WithMessage($"{nameof(UpdateNotificationLevelHierarchyCommand.NotificationConfigId)} {rule.Error}")
                        .MustAsync(async (command, notificationConfigId, cancellation) =>
                            !await _NotificationLevelHierarchyCommandRepository
                                .IsNameDuplicateAsync(notificationConfigId, command.TargetTypeId, command.TargetId,command.Id))
                        .WithMessage("The combination already exists.");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (Id, cancellation) =>
                                await _NotificationLevelHierarchyQueryRepository.NotFoundAsync(Id))
                            .WithName("Id")
                            .WithMessage($"{rule.Error}");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}