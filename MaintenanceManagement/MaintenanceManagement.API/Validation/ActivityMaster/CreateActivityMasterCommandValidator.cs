using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Command.CreateActivityMaster;
using Core.Application.Common.Interfaces.IActivityMaster;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;
using MaintenanceManagement.Infrastructure.Migrations;

namespace MaintenanceManagement.API.Validation.ActivityMaster
{
    public class CreateActivityMasterCommandValidator  : AbstractValidator<CreateActivityMasterCommand>
    {
        private readonly List<ValidationRule> _validationRules;            
      private readonly IActivityMasterQueryRepository _activityMasterQueryRepository;

      public CreateActivityMasterCommandValidator( IActivityMasterQueryRepository activityMasterQueryRepository,MaxLengthProvider maxLengthProvider)
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
                        RuleFor(x => x.CreateActivityMasterDto.ActivityName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateActivityMasterCommand.CreateActivityMasterDto.ActivityName)} {rule.Error}");
                        break;
                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.CreateActivityMasterDto.ActivityName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateActivityMasterCommand.CreateActivityMasterDto.ActivityName)} {rule.Error}");
                        break;
                    //    case "AlreadyExists":
                    //         RuleFor(x => x.CreateActivityMasterDto.ActivityName)
                    //             .MustAsync(async (activityname, cancellation) =>
                    //                 !await _activityMasterQueryRepository.GetByActivityNameAsync(activityname ))
                    //             .WithMessage("Activity name already exists.");
                    //         break;   

                    case "AlreadyExists":
                                                RuleFor(x => x.CreateActivityMasterDto.ActivityName)
                        .MustAsync(async (activityName, _) =>
                            !await _activityMasterQueryRepository.GetByActivityNameAsync(activityName ))
                        .WithMessage("Activity name already exists.");
                     break; 


          
                }
            }
      }
    }
}