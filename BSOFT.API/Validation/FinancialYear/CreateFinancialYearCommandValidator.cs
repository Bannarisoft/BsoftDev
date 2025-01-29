using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.API.Validation.Common;
using Core.Application.FinancialYear.Command.CreateFinancialYear;
using FluentValidation;

namespace BSOFT.API.Validation.FinancialYear
{
    public class CreateFinancialYearCommandValidator  : AbstractValidator<CreateFinancialYearCommand>
    {
        private readonly List<ValidationRule> _validationRules;

            public CreateFinancialYearCommandValidator(MaxLengthProvider maxLengthProvider )
            {
            
                  var FinancialYearStartYearMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.FinancialYear>("StartYear") ?? 50;

                    _validationRules = ValidationRuleLoader.LoadValidationRules();
                   if (_validationRules == null || !_validationRules.Any())
                   {
                       throw new InvalidOperationException("Validation rules could not be loaded.");
                   }

                    foreach (var rule in _validationRules)
                    {
                        switch(rule.Rule)
                       { 
                        
                       }
                    }



            }

    }
}