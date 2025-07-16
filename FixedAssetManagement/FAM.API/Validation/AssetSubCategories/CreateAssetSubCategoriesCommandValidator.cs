using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetSubCategories.Command.CreateAssetSubCategories;
using FAM.API.Validation.Common;
using FluentValidation;
using Serilog;

namespace FAM.API.Validation.AssetSubCategories
{
    public class CreateAssetSubCategoriesCommandValidator : AbstractValidator<CreateAssetSubCategoriesCommand>
    {
        private readonly List<ValidationRule> _validationRules;
        public CreateAssetSubCategoriesCommandValidator(MaxLengthProvider maxLengthProvider)
        {
            var CodeMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetSubCategories>("Code") ?? 10;
            var CategoryNameMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetSubCategories>("SubCategoryName") ?? 50;
            var CategoryDescriptionMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetSubCategories>("Description") ?? 250;
            var CategoryIdMaxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.AssetSubCategories>("AssetCategoriesId") ?? 4;
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
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.Code)} {rule.Error}");
                        RuleFor(x => x.SubCategoryName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.SubCategoryName)} {rule.Error}");
                        RuleFor(x => x.AssetCategoriesId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.AssetCategoriesId)} {rule.Error}");
                        break;
                    case "MaxLength":
                        RuleFor(x => x.Code)
                            .MaximumLength(CodeMaxLength)
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.Code)} {rule.Error} {CodeMaxLength}");
                        RuleFor(x => x.SubCategoryName)
                            .MaximumLength(CategoryNameMaxLength)
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.SubCategoryName)} {rule.Error} {CategoryNameMaxLength}");
                        RuleFor(x => x.Description)
                            .MaximumLength(CategoryDescriptionMaxLength)
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.Description)} {rule.Error} {CategoryDescriptionMaxLength}");
                        RuleFor(x => x.AssetCategoriesId.ToString())
                            .MaximumLength(CategoryIdMaxLength)
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.AssetCategoriesId)} {rule.Error} {CategoryIdMaxLength}");
                        break;
                    case "AlphanumericOnly":
                              RuleFor(x => x.Code)
                             .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern)) 
                             .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.Code)} {rule.Error}");   
                        break;
                    case "AlphaNumericWithPunctuation":
                        RuleFor(x => x.SubCategoryName)
                            .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                            .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.SubCategoryName)} {rule.Error}");
                        break;
                    case "AlphabeticOnly":
                         RuleFor(x => x.Description)
                        .Matches(new System.Text.RegularExpressions.Regex(rule.Pattern))
                        .When(x => !string.IsNullOrEmpty(x.Description))
                        .WithMessage($"{nameof(CreateAssetSubCategoriesCommand.Description)} {rule.Error}");
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

