using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MiscMaster.Command.CreateMiscMaster;
using MaintenanceManagement.API.Validation.Common;
using FluentValidation;
using Core.Application.Common.Interfaces.IMiscMaster;

namespace MaintenanceManagement.API.Validation.MiscMaster
{
    public class CreateMiscMasterCommandValidator  : AbstractValidator<CreateMiscMasterCommand>
    {
            private readonly List<ValidationRule> _validationRules;
            private readonly IMiscMasterQueryRepository _miscMasterQuery;

         public CreateMiscMasterCommandValidator( IMiscMasterQueryRepository miscMasterQuery,MaxLengthProvider maxLengthProvider)
        {

           

            var MiscCodeMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.MiscMaster>("Code") ?? 50;
            var DescriptionMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.MiscMaster>("Description")?? 250;

            _validationRules = ValidationRuleLoader.LoadValidationRules();
             _miscMasterQuery = miscMasterQuery;
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
                        RuleFor(x => x.Code)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMiscMasterCommand.Code)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMiscMasterCommand.Description)} {rule.Error}");
                              RuleFor(x => x.MiscTypeId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateMiscMasterCommand.MiscTypeId)} {rule.Error}");
                        break;
                        case "MaxLength" :
                        RuleFor(x => x.Code)
                           
                            .MaximumLength(MiscCodeMaxLength)
                            .WithMessage($"{nameof(CreateMiscMasterCommand.Code)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .MaximumLength(DescriptionMaxLength)
                            .WithMessage($"{nameof(CreateMiscMasterCommand.Description)} {rule.Error}");
                        break;
                        case "AlreadyExists":
                           RuleFor(x => x.Code)
                           .MustAsync(async (Code, cancellation) => !await _miscMasterQuery.AlreadyExistsAsync(Code))
                           .WithName("Misc Code")
                            .WithMessage($"{rule.Error}");
                        break;
                        
                }

            }
       

        }
        
    }
}