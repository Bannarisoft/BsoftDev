using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Item.ItemCategory.Commands.UpdateItemCategory;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemCategory
{
    public class UpdateItemCategoryCommandValidator : AbstractValidator<UpdateItemCategoryCommand>
    {        
        private readonly List<ValidationRule> _validationRules;                    
        private readonly IItemCategoryCommandRepository _itemCategoryCommandRepository;
        private readonly IItemCategoryQueryRepository _itemCategoryQueryRepository;  
        public UpdateItemCategoryCommandValidator(MaxLengthProvider maxLengthProvider, IItemCategoryCommandRepository itemCategoryCommandRepository,IItemCategoryQueryRepository itemCategoryQueryRepository)
        {
            _itemCategoryCommandRepository = itemCategoryCommandRepository;            
            _itemCategoryQueryRepository = itemCategoryQueryRepository;
            var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Item.ItemCategory>("ItemCategoryName") ?? 250;

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
                        RuleFor(x => x.ItemCategoryName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateItemCategoryCommand.ItemCategoryName)} {rule.Error}");
                        RuleFor(x => x.ItemGroupId)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateItemCategoryCommand.ItemGroupId)} {rule.Error}");
                        break;
                    case "MaxLength":
                         RuleFor(x => x.ItemCategoryName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(UpdateItemCategoryCommand.ItemCategoryName)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                          RuleFor(x => x.ItemCategoryName)
                           .NotEmpty()
                           .WithMessage($"{nameof(UpdateItemCategoryCommand.ItemCategoryName)} {rule.Error}")
                           .MustAsync(async (command, moduleName, cancellation) =>
                            !await _itemCategoryCommandRepository.IsNameDuplicateAsync(moduleName, command.ItemGroupId,command.Id))
                             .WithMessage("A Category Name already exists in this Group.");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (Id, cancellation) =>
                                await _itemCategoryQueryRepository.NotFoundAsync(Id))
                            .WithName("Id")
                            .WithMessage($"{rule.Error}");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}