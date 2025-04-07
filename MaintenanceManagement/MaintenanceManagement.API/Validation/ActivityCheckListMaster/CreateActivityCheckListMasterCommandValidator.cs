using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Command.CreateActivityCheckListMaster;
using Core.Application.Common.Interfaces.IActivityCheckListMaster;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.ActivityCheckListMaster
{
    public class CreateActivityCheckListMasterCommandValidator  : AbstractValidator<CreateActivityCheckListMasterCommand>
    {
         private readonly List<ValidationRule> _validationRules;            
      private readonly IActivityCheckListMasterQueryRepository _activityCheckListMasterQueryRepository;


       public CreateActivityCheckListMasterCommandValidator( IActivityCheckListMasterQueryRepository  activityCheckListMasterQueryRepository,MaxLengthProvider maxLengthProvider)
      {
          var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.ActivityCheckListMaster>("ActivityCheckList") ?? 250;

          _validationRules = new List<ValidationRule>();
             _validationRules = ValidationRuleLoader.LoadValidationRules();
          _activityCheckListMasterQueryRepository = activityCheckListMasterQueryRepository;
           
           foreach (var rule in _validationRules)
            {
                 switch (rule.Rule)
                { 
                  case "NotEmpty":
                   RuleFor(x => x.ActivityCheckList)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateActivityCheckListMasterCommand.ActivityCheckList)} {rule.Error}");
                        break;
                  case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.ActivityCheckList)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateActivityCheckListMasterCommand.ActivityCheckList)} {rule.Error}"); 
                            break;

                    // case "AlreadyExists":
                    //     RuleFor(x => x.ActivityCheckList )
                    //         .MustAsync(async (activityCheckList, cancellation) =>
                    //             !await _activityCheckListMasterQueryRepository.GetByActivityCheckListAsync(activityCheckList.ActivityCheckList,activityCheckList.ActivityID))
                    //         .WithMessage("ActivityCheckList  already exists.");
                    //     break;   
                    case "AlreadyExists":
                        RuleFor(x => x)
                            .MustAsync(async (command, cancellation) =>
                                !await _activityCheckListMasterQueryRepository
                                    .GetByActivityCheckListAsync(command.ActivityCheckList, command.ActivityID))
                            .WithMessage("ActivityCheckList already exists.");
                        break;   
                  default:
                        // Handle unknown rule (log or throw)
                        Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                        break;          



                }
            }                
                   

 

      }

        
    }
}