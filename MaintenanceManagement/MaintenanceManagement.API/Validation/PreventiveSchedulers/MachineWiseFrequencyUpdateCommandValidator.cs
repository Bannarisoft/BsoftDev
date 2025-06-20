using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.MachineWiseFrequencyUpdate;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.PreventiveSchedulers
{
    public class MachineWiseFrequencyUpdateCommandValidator : AbstractValidator<MachineWiseFrequencyUpdateCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public MachineWiseFrequencyUpdateCommandValidator(IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _preventiveSchedulerQuery = preventiveSchedulerQuery;

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
                                   .NotNull()
                                   .WithMessage($"{nameof(MachineWiseFrequencyUpdateCommand.Id)} {rule.Error}")
                                   .NotEmpty()
                                   .WithMessage($"{nameof(MachineWiseFrequencyUpdateCommand.Id)} {rule.Error}");
                        RuleFor(x => x.FrequencyInterval)
                                .NotNull()
                                .WithMessage($"{nameof(MachineWiseFrequencyUpdateCommand.FrequencyInterval)} {rule.Error}")
                                .NotEmpty()
                                .WithMessage($"{nameof(MachineWiseFrequencyUpdateCommand.FrequencyInterval)} {rule.Error}");
                        RuleFor(x => x.LastMaintenanceActivityDate)
                                .Must(date => date != DateOnly.MinValue)
                                .WithMessage($"{nameof(MachineWiseFrequencyUpdateCommand.LastMaintenanceActivityDate)} {rule.Error}")
                                .When(x => x.IsActive ==1);

                        break;

                    case "NotFound":
                        RuleFor(x => x.Id)
                              .MustAsync(async (Id, cancellation) =>
                           await _preventiveSchedulerQuery.NotFoundDetailAsync(Id))
                               .WithMessage($"{rule.Error}");                              
                        break;

                    default:
                        break;
                }
            }
        }
    }
}