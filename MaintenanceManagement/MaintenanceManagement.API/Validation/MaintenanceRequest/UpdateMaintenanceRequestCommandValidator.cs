using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Command.UpdateMaintenanceRequestCommand;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MaintenanceRequest
{
    public class UpdateMaintenanceRequestCommandValidator : AbstractValidator<UpdateMaintenanceRequestCommand>
    {
              private readonly List<ValidationRule> _validationRules;  
        private readonly IMaintenanceRequestQueryRepository  _maintenanceRequestQueryRepository;
        


      public UpdateMaintenanceRequestCommandValidator(IMaintenanceRequestQueryRepository  maintenanceRequestQueryRepository)
      {
         _maintenanceRequestQueryRepository = maintenanceRequestQueryRepository;
             // Load validation rules internally — no injection needed
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
                        RuleFor(x => x.MaintenanceTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateMaintenanceRequestCommand.MaintenanceTypeId)} {rule.Error}");

                        RuleFor(x => x.RequestId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateMaintenanceRequestCommand.RequestId)} {rule.Error}");

                        RuleFor(x => x.MachineId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateMaintenanceRequestCommand.MachineId)} {rule.Error}");

                        RuleFor(x => x.RequestTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateMaintenanceRequestCommand.RequestTypeId)} {rule.Error}");

                        RuleFor(x => x.MachineId)
                            .GreaterThan(0)
                            .WithMessage("MachineId is required.");

                        RuleFor(x => x.RequestTypeId)
                            .GreaterThan(0)
                            .WithMessage("RequestTypeId is required.");

                        RuleFor(x => x.MaintenanceTypeId)
                            .GreaterThan(0)
                            .WithMessage("MaintenanceTypeId is required.");
                        break;

                    default:
                        break;
                }
            }
      }

    }
}