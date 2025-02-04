using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.API.Validation.Common;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using FluentValidation;



namespace UserManagement.API.Validation.FinancialYear
{
    public class CreateFinancialYearCommandValidator  : AbstractValidator<CreateFinancialYearCommand>
    {
        private readonly List<ValidationRule> _validationRules;

            public CreateFinancialYearCommandValidator(MaxLengthProvider maxLengthProvider )
            {
            
                  var FinancialYearStartYearMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.FinancialYear>("StartYear") ?? 50;
                  var FinancialYearEndYearMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.FinancialYear>("FinYearName") ?? 50;

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
                                RuleFor(x => x.StartYear)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateFinancialYearCommand.StartYear)} {rule.Error}");
          
                                RuleFor(x => x.StartYear)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateFinancialYearCommand.StartYear)} {rule.Error}");
                                break;
                            case "MaxLength":
                                RuleFor(x => x.StartYear)
                                    .MaximumLength(FinancialYearStartYearMaxLength) // Dynamic value from MaxLengthProvider
                                    .WithMessage($"{nameof(CreateFinancialYearCommand.StartYear)} {rule.Error} {FinancialYearStartYearMaxLength}");             

                              
                                RuleFor(x => x.FinYearName)
                                    .NotEmpty()
                                    .WithMessage($"{nameof(CreateFinancialYearCommand.FinYearName)} {rule.Error}");
                                break;
                          
                       }

                    }



            }

    }
}