using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.PreventiveSchedulers
{
    public class CreatePreventiveSchedulerCommandValidator : AbstractValidator<CreatePreventiveSchedulerCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        public CreatePreventiveSchedulerCommandValidator(MaxLengthProvider maxLengthProvider, IMachineGroupQueryRepository machineGroupQueryRepository,IMiscMasterQueryRepository miscMasterQueryRepository,IActivityMasterQueryRepository activityMasterQueryRepository)
        {
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _activityMasterQueryRepository = activityMasterQueryRepository;
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
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.MachineGroupId)} {rule.Error}");
                        RuleFor(x => x.DepartmentId)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.DepartmentId)} {rule.Error}");
                        RuleFor(x => x.MaintenanceCategoryId)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.MachineGroupId)} {rule.Error}");
                        RuleFor(x => x.ScheduleId)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.ScheduleId)} {rule.Error}");
                        RuleFor(x => x.FrequencyTypeId)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.FrequencyTypeId)} {rule.Error}");
                        RuleFor(x => x.FrequencyInterval)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.FrequencyInterval)} {rule.Error}");
                        RuleFor(x => x.FrequencyUnitId)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.FrequencyUnitId)} {rule.Error}");
                        RuleFor(x => x.EffectiveDate)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.EffectiveDate)} {rule.Error}");

                         RuleFor(x => x.GraceDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.GraceDays)} {rule.Error}");
                        RuleFor(x => x.ReminderWorkOrderDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.ReminderWorkOrderDays)} {rule.Error}");
                        RuleFor(x => x.ReminderMaterialReqDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.ReminderMaterialReqDays)} {rule.Error}");
                        RuleFor(x => x.IsDownTimeRequired)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.IsDownTimeRequired)} {rule.Error}");
                        RuleFor(x => x.DownTimeEstimateHrs)
                                .NotEmpty()
                                .WithMessage($"{nameof(CreatePreventiveSchedulerCommand.DownTimeEstimateHrs)} {rule.Error}");
                        RuleFor(x => x.Activity)
                            .NotNull()
                            .WithMessage($"{rule.Error}")
                            .Must(x => x.Count > 0)
                            .WithMessage($"{rule.Error}");
                    break;
                case "FKColumnDelete":
                        RuleFor(x => x.MachineGroupId)
                               .MustAsync(async (MachineGroupId, cancellation) => 
                                 await _machineGroupQueryRepository.FKColumnExistValidation(MachineGroupId))
                                .WithMessage($"{rule.Error}");  
                        RuleFor(x => x.MaintenanceCategoryId)
                               .MustAsync(async (MaintenanceCategoryId, cancellation) => 
                                 await _miscMasterQueryRepository.FKColumnValidation(MaintenanceCategoryId))
                                .WithMessage($"{rule.Error}");  
                        RuleFor(x => x.ScheduleId)
                               .MustAsync(async (ScheduleId, cancellation) => 
                                 await _miscMasterQueryRepository.FKColumnValidation(ScheduleId))
                                .WithMessage($"{rule.Error}");  
                        RuleFor(x => x.FrequencyTypeId)
                               .MustAsync(async (FrequencyTypeId, cancellation) => 
                                 await _miscMasterQueryRepository.FKColumnValidation(FrequencyTypeId))
                                .WithMessage($"{rule.Error}"); 
                        RuleFor(x => x.FrequencyUnitId)
                               .MustAsync(async (FrequencyUnitId, cancellation) => 
                                 await _miscMasterQueryRepository.FKColumnValidation(FrequencyUnitId))
                                .WithMessage($"{rule.Error}");   
                        RuleFor(x => x.Activity)
                          .ForEach(activityRule =>
                          {
                              activityRule.MustAsync(async (activity, cancellation) => 
                                  await _activityMasterQueryRepository.FKColumnExistValidation(activity.ActivityId))
                                  .WithMessage($"{rule.Error}");  
                          }); 

                    break;
                    case "DateValidation":
                     RuleFor(x => x.EffectiveDate)
                                .Must(BeAValidDate)
                                .WithMessage($"{rule.Error}"); 
                    break;
                    case "PastDateValidation":
                     RuleFor(x => x.EffectiveDate)
                                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                                .WithMessage($"{rule.Error}"); 
                    break;
                    default:                        
                        break;
                }
            }
        }
         private bool BeAValidDate(DateOnly date)
        {
        
            return date >= DateOnly.FromDateTime(DateTime.Today);
        }
    }
}