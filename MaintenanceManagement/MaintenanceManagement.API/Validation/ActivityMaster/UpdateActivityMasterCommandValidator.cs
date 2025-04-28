using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;
using Core.Application.Common.Interfaces.IActivityMaster;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.ActivityMaster
{
    public class UpdateActivityMasterCommandValidator : AbstractValidator<UpdateActivityMasterCommand>
    {
      private readonly List<ValidationRule> _validationRules;            
      private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;
      public UpdateActivityMasterCommandValidator( IActivityMasterQueryRepository activityMasterQueryRepository,MaxLengthProvider maxLengthProvider)
      {
          var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.ActivityMaster>("ActivityName") ?? 250;


          _activityMasterQueryRepository = activityMasterQueryRepository;
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
                        // Apply NotEmpty validation
                        RuleFor(x => x.UpdateActivityMaster.ActivityName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateActivityMasterCommand.UpdateActivityMaster.ActivityName)} {rule.Error}");
                        break;
                  case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.UpdateActivityMaster.ActivityName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(UpdateActivityMasterCommand.UpdateActivityMaster.ActivityName)} {rule.Error}"); 
                            break;
                   case "AlreadyExists":
                        RuleFor(x => x.UpdateActivityMaster.ActivityName)
                            .MustAsync(async (activityname, cancellation) =>
                                !await _activityMasterQueryRepository.GetByActivityNameAsync(activityname))
                            .WithMessage("Activity name already exists.");
                        break;    

          
                }
            }
      }
        
    }
}