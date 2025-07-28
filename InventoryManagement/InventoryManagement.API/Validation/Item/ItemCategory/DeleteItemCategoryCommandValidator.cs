using Core.Application.Common.Interfaces.Item.ItemCategory;
using Core.Application.Item.ItemCategory.Commands.DeleteItemCategory;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemCategory
{
    public class DeleteItemCategoryCommandValidator : AbstractValidator<DeleteItemCategoryCommand> 
    {
        private readonly List<ValidationRule> _validationRules;
        private readonly IItemCategoryQueryRepository _itemCategoryQueryRepository;        

        public DeleteItemCategoryCommandValidator(IItemCategoryQueryRepository itemCategoryQueryRepository)
        {
            _itemCategoryQueryRepository = itemCategoryQueryRepository;            
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
                        RuleFor(x => x.Id)
                            .NotEmpty()
                            .WithMessage($"{nameof(DeleteItemCategoryCommand.Id)} {rule.Error}");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                        .MustAsync(async (Id, cancellation) => 
                        await _itemCategoryQueryRepository.NotFoundAsync(Id))             
                        .WithName("Id")
                        .WithMessage($"{rule.Error}");
                        break;
                    case "SoftDelete":
                        RuleFor(x => x.Id)
                        .MustAsync(async (Id, cancellation) => !await _itemCategoryQueryRepository.SoftDeleteValidation(Id))
                        .WithMessage($"{rule.Error}");
                        break;
                    default:                        
                        break;
                }
            }
        }
    }
}