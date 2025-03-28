using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.MachineGroup.Command.CreateMachineGroup;
using FluentValidation;
using MaintenanceManagement.API.Validation.Common;

namespace MaintenanceManagement.API.Validation.MachineGroup
{
    public class CreateMachineGroupCommandValidator : AbstractValidator<CreateMachineGroupCommand>
    {
      private readonly List<ValidationRule> _validationRules;            
      private readonly IMachineGroupQueryRepository _machineGroupQueryRepository;   
        
        public CreateMachineGroupCommandValidator(IMachineGroupQueryRepository machineGroupQueryRepository ,MaxLengthProvider maxLengthProvider)
        {
           var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.MachineGroup>("GroupName") ?? 50;

             _machineGroupQueryRepository = machineGroupQueryRepository;
        

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
                        // Apply NotEmpty validation
                        RuleFor(x => x.GroupName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMachineGroupCommand.GroupName)} {rule.Error}");
                        break;
                  case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.GroupName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateMachineGroupCommand.GroupName)} {rule.Error}");
                            break;
                            case "AlphanumericOnly": 
                        RuleFor(x => x.GroupName)
                            .Matches(new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9 ]+$")) // Allow spaces
                             .WithMessage($"{nameof(CreateMachineGroupCommand.GroupName)} {rule.Error}");
                        break;
                         case "AlphaNumericWithPunctuation":
                        RuleFor(x => x.GroupName)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateMachineGroupCommand.GroupName)} {rule.Error}");
                        break;  
                 
                  case "AlreadyExists":
                        RuleFor(x => x.GroupName)
                            .MustAsync(async (groupName, cancellation) =>
                                !await _machineGroupQueryRepository.GetByMachineGroupnameAsync(groupName))
                            .WithMessage("Group name already exists.");
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