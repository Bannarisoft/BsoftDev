using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using MaintenanceManagement.API.Validation.Common;
using FluentValidation;

namespace MaintenanceManagement.API.Validation.MiscTypeMaster
{
    public class UpdateMiscTypeMasterCommandValidator  : AbstractValidator<UpdateMiscTypeMasterCommand>
    {

          private readonly List<ValidationRule> _validationRules;
          public UpdateMiscTypeMasterCommandValidator(MaxLengthProvider maxLengthProvider)
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
                            .WithMessage($"{nameof(UpdateMiscTypeMasterCommand.MiscTypeCode)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateMiscTypeMasterCommand.Description)} {rule.Error}");
                        break;
                        case "MaxLength" :
                        RuleFor(x => x.MiscTypeCode)
                           
                            .MaximumLength(MiscTypeCodeMaxLength)
                            .WithMessage($"{nameof(UpdateMiscTypeMasterCommand.MiscTypeCode)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .MaximumLength(DescriptionMaxLength)
                            .WithMessage($"{nameof(UpdateMiscTypeMasterCommand.Description)} {rule.Error}");
                        break;
                     
                }

           }
          }
        
    }
}