using BackgroundService.API.Validation.Common;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy;
using FluentValidation;

namespace BackgroundService.API.Validation.NotificationLevelHierarchy
{
    public class DeleteNotificationLevelHierarchyCommandValidator : AbstractValidator<DeleteNotificationLevelHierarchyCommand> 
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly INotificationLevelHierarchyQueryRepository _NotificationLevelHierarchyQueryRepository;        

        public DeleteNotificationLevelHierarchyCommandValidator(INotificationLevelHierarchyQueryRepository NotificationLevelHierarchyQueryRepository)
        {
            _NotificationLevelHierarchyQueryRepository = NotificationLevelHierarchyQueryRepository;            
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
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteNotificationLevelHierarchyCommand.Id)} {rule.Error}");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (Id, cancellation) => 
                                await _NotificationLevelHierarchyQueryRepository.NotFoundAsync(Id))             
                            .WithName("Id")
                            .WithMessage($"{rule.Error}");
                            break;
                    case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _NotificationLevelHierarchyQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:                        
                        break;
                }
            }
        }
    }
}