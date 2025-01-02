using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Core.Application.Departments.Commands.CreateDepartment;
using Core.Application.Departments.Commands.UpdateDepartment;
using BSOFT.API.Validation.Common;

namespace BSOFT.API.Validation.Department
{
    public class UpdateDepartmentCommandValidator :AbstractValidator<UpdateDepartmentCommand>
    {

         private readonly List<ValidationRule> _validationRules;


     public UpdateDepartmentCommandValidator(MaxLengthProvider maxLengthProvider)
           {
                    var DepartmentShortNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Department>("ShortName") ?? 6;
                   var DepartmentDeptNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Department>("DeptName") ?? 50; 
                    _validationRules = ValidationRuleLoader.LoadValidationRules();
                   if (_validationRules == null || !_validationRules.Any())
                   {
                       throw new InvalidOperationException("Validation rules could not be loaded.");
                   }

                   // Loop through the rules and apply them
                   foreach (var rule in _validationRules)
                   {
                       switch(rule.Rule)
                       {
                        case "NotEmpty":
                            // Apply NotEmpty validation
                            RuleFor(x => x.DeptName).MaximumLength(DepartmentDeptNameMaxLength)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdateDepartmentCommand.DeptName)} {rule.Error}");
                            RuleFor(x => x.ShortName).MaximumLength(DepartmentShortNameMaxLength)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdateDepartmentCommand.ShortName)} {rule.Error}");
                            break;

                            case "MaxLength":
                            // Apply MaxLength validation using dynamic max length values
                            RuleFor(x => x.DeptName)
                                .MaximumLength(DepartmentDeptNameMaxLength)
                                .WithMessage($"{nameof(UpdateDepartmentCommand.DeptName)} {rule.Error} {DepartmentDeptNameMaxLength}");
                            RuleFor(x => x.ShortName)
                            .MaximumLength(DepartmentDeptNameMaxLength)
                            .WithMessage($"{nameof(UpdateDepartmentCommand.ShortName)} {rule.Error} {DepartmentShortNameMaxLength}"); break;
                            default:
                               // Handle unknown rule (log or throw)
                               Console.WriteLine($"Warning: Unknown rule '{rule.Rule}' encountered.");
                            break;

                       }
                   }


           }

        
    }
}