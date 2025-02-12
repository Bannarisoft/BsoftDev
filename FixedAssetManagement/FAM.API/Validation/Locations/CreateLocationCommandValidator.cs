using FAM.API.Validation.Common;
using Core.Domain.Entities;
using FluentValidation;
using Core.Application.Location.Command.CreateLocation;

namespace FAM.API.Validation.Locations
{
    public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateLocationCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var LocationNameMaxLength = maxLengthProvider.GetMaxLength<Location>("LocationName") ?? 50;
            var DescriptionMaxLength = maxLengthProvider.GetMaxLength<Location>("Description") ?? 100;
            
            _validationRules = ValidationRuleLoader.LoadValidationRules();
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
                        RuleFor(x => x.Code)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLocationCommand.Code)} {rule.Error}");
                        RuleFor(x => x.LocationName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLocationCommand.LocationName)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLocationCommand.Description)} {rule.Error}");
                        RuleFor(x => x.SortOrder)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateLocationCommand.SortOrder)} {rule.Error}");
                        break;
                    case "MaxLength":
                        // Apply MaxLength validation using dynamic max length values
                        RuleFor(x => x.LocationName)
                            .MaximumLength(LocationNameMaxLength)
                            .WithMessage($"{nameof(CreateLocationCommand.LocationName)} {rule.Error}");
                        RuleFor(x => x.Description)
                            .MaximumLength(DescriptionMaxLength)
                            .WithMessage($"{nameof(CreateLocationCommand.Description)} {rule.Error}");
                        break;
                        case "MinLength":
                        RuleFor(x => x.UnitId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateLocationCommand.UnitId)} {rule.Error} {0}");   
                            RuleFor(x => x.DepartmentId)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateLocationCommand.DepartmentId)} {rule.Error} {0}");
                            RuleFor(x => x.SortOrder)
                            .GreaterThanOrEqualTo(1)
                            .WithMessage($"{nameof(CreateLocationCommand.SortOrder)} {rule.Error} {0}");
                        break; 
                }
            }
        }
    }
}