
using Core.Application.Common.Interfaces.IMachineGroupUser;
using Core.Application.MachineGroupUsers.Command.CreateMachineGroupUser;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MachineGroupUser
{
    public class CreateMachineGroupUserCommandValidator  : AbstractValidator<CreateMachineGroupUserCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IMachineGroupUserQueryRepository _machineGroupUserQuery;
        public CreateMachineGroupUserCommandValidator(MaxLengthProvider maxLengthProvider,IMachineGroupUserQueryRepository machineGroupUserQuery)
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
                        RuleFor(x => x.MachineGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMachineGroupUserCommand.MachineGroupId)} {rule.Error}");
                        RuleFor(x => x.DepartmentId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMachineGroupUserCommand.DepartmentId)} {rule.Error}");
                        RuleFor(x => x.UserId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMachineGroupUserCommand.UserId)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                           RuleFor(x => x.MachineGroupId)
                           .MustAsync(async (MachineGroupId, cancellation) => !await _machineGroupUserQuery.AlreadyExistsAsync(MachineGroupId))
                           .WithName("Shift Name")
                            .WithMessage($"{rule.Error}");
                            break;                    
                      default:                        
                    break;
                }
            }
        }
       
    }
}