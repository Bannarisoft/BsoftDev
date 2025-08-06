using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationLevelHierarchy
{
    public class CreateNotificationLevelHierarchyCommandValidator  : AbstractValidator<CreateNotificationLevelHierarchyCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;                    
        private readonly INotificationLevelHierarchyCommandRepository _NotificationLevelHierarchyCommandRepository;

        public CreateNotificationLevelHierarchyCommandValidator(MaxLengthProvider maxLengthProvider,INotificationLevelHierarchyCommandRepository NotificationLevelHierarchyCommandRepository)
        {
            var maxLength = maxLengthProvider.GetMaxLength<Domain.Entities.Notification.NotificationLevelHierarchy>("Description") ?? 250;
            
            _NotificationLevelHierarchyCommandRepository = NotificationLevelHierarchyCommandRepository;
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
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.Description)} {rule.Error}");
                        RuleFor(x => x.NotificationConfigId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.NotificationConfigId)} {rule.Error}");
                        RuleFor(x => x.TargetTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.TargetTypeId)} {rule.Error}");
                        RuleFor(x => x.TargetId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.TargetId)} {rule.Error}");
                        RuleFor(x => x.ApprovalModeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.ApprovalModeId)} {rule.Error}");                       
                        break;
                    case "MaxLength":                        
                        RuleFor(x => x.Description)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.Description)} {rule.Error}");
                        break;
                    case "AlreadyExists":                       
                     RuleFor(x => x.NotificationConfigId)
                        .NotEmpty()
                        .WithMessage($"{nameof(CreateNotificationLevelHierarchyCommand.NotificationConfigId)} {rule.Error}")
                        .MustAsync(async (command, notificationConfigId, cancellation) =>
                            !await _NotificationLevelHierarchyCommandRepository
                                .ExistsByCodeAsync(notificationConfigId, command.TargetTypeId, command.TargetId))
                        .WithMessage("The combination already exists.");
                        break;



                }
            }
        }
    }
}