using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Core.Application.UserRole.Commands.CreateRole;
using BSOFT.API.Validation.Common;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace BSOFT.API.Validation.UserRole
{

     
    public class CreateRoleCommandValidator  : AbstractValidator<CreateRoleCommand>
    {

     private readonly List<ValidationRule> _validationRules;

          public CreateRoleCommandValidator(MaxLengthProvider maxLengthProvider)
           {

                    var DepartmentShortNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.UserRole>("RoleName") ?? 50;
                   var DepartmentDeptNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.UserRole>("Description") ?? 250;            
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
                               RuleFor(x => x.RoleName).MaximumLength(DepartmentShortNameMaxLength)
                                   .NotEmpty()
                                   .WithMessage($"{nameof(CreateRoleCommand.RoleName)} {rule.Error}");
                               RuleFor(x => x.Description).MaximumLength(DepartmentDeptNameMaxLength)
                                   .NotEmpty()
                                   .WithMessage($"{nameof(CreateRoleCommand.Description)} {rule.Error}");
                               break;
                           case "MaxLength":
                               // Apply MaxLength validation using dynamic max length values
                               RuleFor(x => x.RoleName).MaximumLength(DepartmentShortNameMaxLength)
                                   .WithMessage($"{nameof(CreateRoleCommand.RoleName)} {rule.Error}");
                               RuleFor(x => x.Description).MaximumLength(DepartmentDeptNameMaxLength)
                                   .WithMessage($"{nameof(CreateRoleCommand.Description)} {rule.Error}");
                               break;
                       }
                   }


           }
        
    }
}