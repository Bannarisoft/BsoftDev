using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IActivityMaster;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.PreventiveSchedulers
{
    public class UpdatePreventiveSchedulerCommandValidator : AbstractValidator<UpdatePreventiveSchedulerCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public UpdatePreventiveSchedulerCommandValidator(IMachineGroupQueryRepository machineGroupQueryRepository,IMiscMasterQueryRepository miscMasterQueryRepository,IActivityMasterQueryRepository activityMasterQueryRepository,IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
             _validationRules = ValidationRuleLoader.LoadValidationRules();
            _machineGroupQueryRepository = machineGroupQueryRepository;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _activityMasterQueryRepository = activityMasterQueryRepository;
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
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.Id)} {rule.Error}");
                        RuleFor(x => x.MachineGroupId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.MachineGroupId)} {rule.Error}");
                        RuleFor(x => x.DepartmentId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.DepartmentId)} {rule.Error}");
                        RuleFor(x => x.MaintenanceCategoryId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.MaintenanceCategoryId)} {rule.Error}");
                        RuleFor(x => x.ScheduleId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.ScheduleId)} {rule.Error}");
                        RuleFor(x => x.FrequencyTypeId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.FrequencyTypeId)} {rule.Error}");
                        RuleFor(x => x.FrequencyInterval)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.FrequencyInterval)} {rule.Error}");
                        RuleFor(x => x.FrequencyUnitId)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.FrequencyUnitId)} {rule.Error}");
                        RuleFor(x => x.EffectiveDate)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.EffectiveDate)} {rule.Error}");

                         RuleFor(x => x.GraceDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.GraceDays)} {rule.Error}");
                        RuleFor(x => x.ReminderWorkOrderDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.ReminderWorkOrderDays)} {rule.Error}");
                        RuleFor(x => x.ReminderMaterialReqDays)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.ReminderMaterialReqDays)} {rule.Error}");
                        RuleFor(x => x.IsDownTimeRequired)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.IsDownTimeRequired)} {rule.Error}");
                        RuleFor(x => x.DownTimeEstimateHrs)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdatePreventiveSchedulerCommand.DownTimeEstimateHrs)} {rule.Error}");
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
                    case "NotFound":
                     RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _preventiveSchedulerQuery.NotFoundAsync(Id))
                            .WithMessage($"{rule.Error}");
                            break;  
                  case "WorkOrderValidate":
                     RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _preventiveSchedulerQuery.UpdateValidation(Id))
                            .WithMessage($"{rule.Error}");
                            break;  
                  case "AlreadyExists":
                        RuleForEach(x => x.Activity)
                         .MustAsync(async (command, activityDto, context, cancellation) =>
                         {
                             return !await _preventiveSchedulerQuery.AlreadyExistsAsync(
                                 activityDto.ActivityId, 
                                 command.MachineGroupId,        
                                 command.Id                     
                             );
                         }) 
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