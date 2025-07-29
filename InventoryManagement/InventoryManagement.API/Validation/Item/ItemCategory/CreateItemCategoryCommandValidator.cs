using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Item.ItemCategory.Commands.CreateItemCategory;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemCategory
{
    public class CreateItemCategoryCommandValidator  : AbstractValidator<CreateItemCategoryCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;                    
        private readonly IItemCategoryCommandRepository _itemCategoryCommandRepository;

        public CreateItemCategoryCommandValidator(MaxLengthProvider maxLengthProvider,IItemCategoryCommandRepository itemCategoryCommandRepository)
        {
            var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Item.ItemCategory>("ItemCategoryName") ?? 100;
            
            _itemCategoryCommandRepository = itemCategoryCommandRepository;
            _validationRules = ValidationRuleLoader.LoadValidationRules();

            if (_validationRules == null || !_validationRules.Any())
            {
                throw new ArgumentException("Validation rules could not be loaded.");
            }

            foreach (var rule in _validationRules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.ItemCategoryName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateItemCategoryCommand.ItemCategoryName)} {rule.Error}");
                        RuleFor(x => x.ItemGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateItemCategoryCommand.ItemGroupId)} {rule.Error}");
                        break;
                    case "MaxLength":                        
                        RuleFor(x => x.ItemCategoryName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateItemCategoryCommand.ItemCategoryName)} {rule.Error}");
                        break;
                    case "AlreadyExists":                       
                        RuleFor(x => x.ItemCategoryName)
                           .NotEmpty()
                           .WithMessage($"{nameof(CreateItemCategoryCommand.ItemCategoryName)} {rule.Error}")
                           .MustAsync(async (command, moduleName, cancellation) =>
                            !await _itemCategoryCommandRepository.ExistsByNameAsync(moduleName,command.ItemGroupId))
                             .WithMessage("A Category Name already exists in this Group.");
                        break;
                }
            }
        }
    }
}