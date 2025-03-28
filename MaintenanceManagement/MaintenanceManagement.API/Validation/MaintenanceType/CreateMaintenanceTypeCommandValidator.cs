using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Core.Application.Common.Interfaces.IMaintenanceType;
using Core.Application.MaintenanceType.Command.CreateMaintenanceType;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MaintenanceType
{
    public class CreateMaintenanceTypeCommandValidator : AbstractValidator<CreateMaintenanceTypeCommand> 
    {
          private readonly List<ValidationRule> _validationRules;
         private readonly IMaintenanceTypeCommandRepository _imaintenanceTypeCommandRepository;
        public CreateMaintenanceTypeCommandValidator(IMaintenanceTypeCommandRepository imaintenanceTypeCommandRepository)
        {
            _imaintenanceTypeCommandRepository = imaintenanceTypeCommandRepository;
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
                        RuleFor(x => x.TypeName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMaintenanceTypeCommand.TypeName)} {rule.Error}");
                        break;
                    case "AlphaNumericWithPunctuation":
                        RuleFor(x => x.TypeName)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateMaintenanceTypeCommand.TypeName)} {rule.Error}");
                        break;
                     case "AlreadyExists":
                            RuleFor(x => x.TypeName)
                           .MustAsync(async (TypeName, cancellation) => !await _imaintenanceTypeCommandRepository.ExistsByCodeAsync(TypeName))
                           .WithName("MachineTypeName")
                           .WithMessage($"{rule.Error}");
                            break; 
    
                }
            }
        }

    }
}