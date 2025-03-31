using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Application.MachineGroupUser.Command.DeleteMachineGroupUser;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MachineGroupUser
{
    public class DeleteMachineGroupUserCommandValidator  : AbstractValidator<DeleteMachineGroupUserCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        
        private readonly IMachineGroupUserQueryRepository _machineGroupUserQuery;
        public DeleteMachineGroupUserCommandValidator(IMachineGroupUserQueryRepository machineGroupUserQuery)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _machineGroupUserQuery =machineGroupUserQuery;
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
                            .WithMessage($"{nameof(DeleteMachineGroupUserCommand.Id)} {rule.Error}");
                        break;
                   /*  case "SoftDelete":
                         RuleFor(x => x.Id)
                      .MustAsync(async (Id, cancellation) => !await _machineGroupUserQuery.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break; */
                        case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _machineGroupUserQuery.NotFoundAsync(Id))             
                           .WithName("Machine Group Id")
                            .WithMessage($"{rule.Error}");
                            break; 
                    default:                        
                    break;
                }
            }
        }
    }
}