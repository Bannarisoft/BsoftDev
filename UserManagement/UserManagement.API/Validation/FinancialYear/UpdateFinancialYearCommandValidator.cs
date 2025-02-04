using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Validation.Common;
using Core.Application.FinancialYear.Command.UpdateFinancialYear;
using FluentValidation;

namespace UserManagement.API.Validation.FinancialYear
{
    public class UpdateFinancialYearCommandValidator :AbstractValidator<UpdateFinancialYearCommand>
    {
             private readonly List<ValidationRule> _validationRules;

        public UpdateFinancialYearCommandValidator( MaxLengthProvider maxLengthProvider)
        {

             var DepartmentStartDateMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.FinancialYear>("StartDate") ?? 50;
            var DepartmentEndDateMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.FinancialYear>("EndDate") ?? 50; 

                  _validationRules = ValidationRuleLoader.LoadValidationRules();
                   if (_validationRules == null || !_validationRules.Any())
                   {
                       throw new InvalidOperationException("Validation rules could not be loaded.");
                   }

                   foreach (var rule in _validationRules)
                   {
                       switch(rule.Rule)
                       {
                           case "NotEmpty":
                            // Apply NotEmpty validation
                            RuleFor(x => x.StartYear).MaximumLength(DepartmentStartDateMaxLength)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdateFinancialYearCommand.StartYear)} {rule.Error}");
                            RuleFor(x => x.FinYearName).MaximumLength(DepartmentStartDateMaxLength)
                                .NotEmpty()
                                .WithMessage($"{nameof(UpdateFinancialYearCommand.FinYearName)} {rule.Error}");
                               
                            break;

                            case "MaxLength":
                            // Apply MaxLength validation using dynamic max length values
                            RuleFor(x => x.StartYear).MaximumLength(DepartmentStartDateMaxLength)
                                .WithMessage($"{nameof(UpdateFinancialYearCommand.StartYear)} {rule.Error} {DepartmentStartDateMaxLength}");
                            RuleFor(x => x.FinYearName).MaximumLength(DepartmentEndDateMaxLength)
                                .WithMessage($"{nameof(UpdateFinancialYearCommand.FinYearName)} {rule.Error} {DepartmentEndDateMaxLength}");
                            break;
                            // case "DateRange":
                            // // Apply DateRange validation
                            // RuleFor(x => x.StartYear).Must((command, startYear) => StartDate >= command.EndDate).WithMessage($"{nameof(UpdateFinancialYearCommand.StartYear)} should be greater than or equal to {nameof(UpdateFinancialYearCommand.EndDate)}."); break;




                       }



            }



            
        }



    }
}