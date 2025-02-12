using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using FAM.API.Validation.Common;
using FAM.Infrastructure.Data;
using FluentValidation;

namespace FAM.API.Validation.MiscTypeMaster
{
    public class CreateMiscTypeMasterCommandValidator  : AbstractValidator<CreateMiscTypeMasterCommand>
    {


             private readonly List<ValidationRule> _validationRules;
      public CreateMiscTypeMasterCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var MiscTypeCodeMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.MiscTypeMaster>("MiscTypeCode") ?? 50;
            var DescriptionMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.MiscTypeMaster>("Description")?? 250;
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
            {
                      throw new InvalidOperationException("Validation rules could not be loaded.");
            }
             foreach (var rule in _validationRules)
            {
             switch (rule.Rule)
                {
                    case "NotFound" :
                     // Apply NotEmpty validation
                        RuleFor(x => x.MiscTypeCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMiscTypeMasterCommand.MiscTypeCode)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMiscTypeMasterCommand.Description)} {rule.Error}");
                        break;
                        case "MaxLength" :
                        RuleFor(x => x.MiscTypeCode)
                           
                            .MaximumLength(MiscTypeCodeMaxLength)
                            .WithMessage($"{nameof(CreateMiscTypeMasterCommand.MiscTypeCode)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .MaximumLength(DescriptionMaxLength)
                            .WithMessage($"{nameof(CreateMiscTypeMasterCommand.Description)} {rule.Error}");
                        break;
                }

            }
       

            }


        
    }
}