using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICustomField;
using Core.Application.CustomFields.Commands.CreateCustomField;
using Core.Domain.Entities;
using FluentValidation;
using UserManagement.API.Validation.Common;

namespace UserManagement.API.Validation.CustomFields
{
    public class CreateCustomFieldCommandValidator : AbstractValidator<CreateCustomFieldCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly ICustomFieldQuery _customFieldQuery;
        public CreateCustomFieldCommandValidator(MaxLengthProvider maxLengthProvider, ICustomFieldQuery customFieldQuery)
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
                        // Apply NotEmpty validation
                        RuleFor(x => x.LabelName)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        RuleFor(x => x.DataTypeId)
                            .NotEmpty()
                            .WithMessage($"{rule.Error}");
                        RuleFor(x => x.LabelTypeId)
                            .NotEmpty()
                            .WithMessage($" {rule.Error}");
                       
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
                           RuleFor(x => x.LabelName)
                           .MustAsync(async (LabelName, cancellation) => !await _customFieldQuery.AlreadyExistsAsync(LabelName))
                           .WithName("Label Name")
                            .WithMessage($"{rule.Error}");
                            break;
                        default:
                        
                        break;
                }
            }
        }
    }
}