using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetCategories.Command.CreateAssetCategories;
using FAM.API.Validation.Common;
using FluentValidation;
using Serilog;

namespace FAM.API.Validation.AssetCategories
{
    public class CreateAssetCategoriesCommandValidator : AbstractValidator<CreateAssetCategoriesCommand>
    {
        private readonly List<ValidationRule> _validationRules;

        public CreateAssetCategoriesCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var CodeMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetCategories>("Code") ?? 10;
            var CategoryNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetCategories>("CategoryName") ?? 50;
            var CategoryDescriptionMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetCategories>("Description") ?? 250;
            var CategoryIdMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetCategories>("AssetGroupId") ?? 4;
              // Load validation rules from JSON or another source
            _validationRules = ValidationRuleLoader.LoadValidationRules();
            if (_validationRules == null || !_validationRules.Any())
            {
                throw new InvalidOperationException("Validation rules could not be loaded.");
            }

            // Loop through the rules and apply them
            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        // Apply NotEmpty validation
                        RuleFor(x => x.Code)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.Code)} {rule.Error}");
                        RuleFor(x => x.CategoryName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.CategoryName)} {rule.Error}");
                        RuleFor(x => x.AssetGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.AssetGroupId)} {rule.Error}");
                        break;
                    case "MaxLength":
                        RuleFor(x => x.Code)
                            .MaximumLength(CodeMaxLength)
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.Code)} {rule.Error} {CodeMaxLength}");
                        RuleFor(x => x.CategoryName)
                            .MaximumLength(CategoryNameMaxLength)
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.CategoryName)} {rule.Error} {CategoryNameMaxLength}");
                        RuleFor(x => x.Description)
                            .MaximumLength(CategoryDescriptionMaxLength)
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.Description)} {rule.Error} {CategoryDescriptionMaxLength}");
                        RuleFor(x => x.AssetGroupId.ToString())
                            .MaximumLength(CategoryIdMaxLength)
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.AssetGroupId)} {rule.Error} {CategoryIdMaxLength}");
                        break;
                    case "AlphanumericOnly":
                              RuleFor(x => x.Code)
                             .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                             .WithMessage($"{nameof(CreateAssetCategoriesCommand.Code)} {rule.Error}");   
                        break;
                    case "AlphaNumericWithPunctuation":
                        RuleFor(x => x.CategoryName)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateAssetCategoriesCommand.CategoryName)} {rule.Error}");
                        break;
                    case "AlphabeticOnly":
                         RuleFor(x => x.Description)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .When(x => !string.IsNullOrEmpty(x.Description))
                        .WithMessage($"{nameof(CreateAssetCategoriesCommand.Description)} {rule.Error}");
                        break;
                    default:
                          // Handle unknown rule (log or throw)
                        Log.Information("Warning: Unknown rule '{Rule}' encountered.", rule.Rule);
                        break;
                }
            }
        }
    }
}