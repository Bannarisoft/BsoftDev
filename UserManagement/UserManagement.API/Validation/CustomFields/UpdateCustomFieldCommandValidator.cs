using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Infrastructure.Migrations;
using Core.Application.Common.Interfaces.ICustomField;
using Core.Application.CustomFields.Commands.UpdateCustomField;
using Core.Domain.Entities;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.CustomFields
{
    public class UpdateCustomFieldCommandValidator : AbstractValidator<UpdateCustomFieldCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly ICustomFieldQuery _customFieldQuery;
        public UpdateCustomFieldCommandValidator(MaxLengthProvider maxLengthProvider, ICustomFieldQuery customFieldQuery)
        {
              var LabelNameMaxLength = maxLengthProvider.GetMaxLength<CustomField>("LabelName") ?? 50;

             _validationRules = ValidationRuleLoader.LoadValidationRules();
             _customFieldQuery = customFieldQuery;
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }
             foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        RuleFor(x => x.LabelName)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        RuleFor(x => x.DataTypeId)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        RuleFor(x => x.LabelTypeId)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        break;
                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.LabelName)
                            .MaximumLength(LabelNameMaxLength)
                            .WithMessage($"{rule.Error}");
                        break;
                        case "MinLength":
                        RuleFor(x => x.DataTypeId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{rule.Error} {0}");
                        RuleFor(x => x.LabelTypeId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{rule.Error} {0}");   
                        break; 
                          case "AlreadyExists":
                           RuleFor(x =>  new { x.LabelName, x.Id })
                           .MustAsync(async (customfield, cancellation) => 
                        !await _customFieldQuery.AlreadyExistsAsync(customfield.LabelName, customfield.Id))             
                           .WithName("Label Name")
                            .WithMessage($"{rule.Error}");
                            break; 

                            case "NotFound":
                           RuleFor(x => x.Id )
                           .MustAsync(async (Id, cancellation) => 
                        await _customFieldQuery.NotFoundAsync(Id))             
                           .WithName("Custom Field Id")
                            .WithMessage($"{rule.Error}");
                            break; 

                    default:
                        
                        break;
                        
                }
            }
        }
        
    }
}