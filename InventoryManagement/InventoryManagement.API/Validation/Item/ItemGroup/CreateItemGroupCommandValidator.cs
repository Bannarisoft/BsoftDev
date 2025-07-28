using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Application.Item.ItemGroup.Commands.CreateItemGroup;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemGroup
{
    public class CreateItemGroupCommandValidator  : AbstractValidator<CreateItemGroupCommand>
    {
        private readonly List<Common.ValidationRule> _validationRules;                    
        private readonly IItemGroupCommandRepository _itemGroupCommandRepository;

        public CreateItemGroupCommandValidator(MaxLengthProvider maxLengthProvider,IItemGroupCommandRepository itemGroupCommandRepository)
        {
            var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Item.ItemGroup>("ItemGroupName") ?? 100;
            
            _itemGroupCommandRepository = itemGroupCommandRepository;
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
                        RuleFor(x => x.ItemGroupName)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateItemGroupCommand.ItemGroupName)} {rule.Error}");
                        RuleFor(x => x.ItemGroupCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(CreateItemGroupCommand.ItemGroupCode)} {rule.Error}");
                        break;
                    case "MaxLength":                        
                        RuleFor(x => x.ItemGroupName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(CreateItemGroupCommand.ItemGroupName)} {rule.Error}");
                        break;
                    case "AlreadyExists":                       
                        RuleFor(x => x.ItemGroupName)
                           .NotEmpty()
                           .WithMessage($"{nameof(CreateItemGroupCommand.ItemGroupName)} {rule.Error}")
                           .MustAsync(async (command, moduleName, cancellation) =>
                            !await _itemGroupCommandRepository.IsNameDuplicateAsync(moduleName,command.Id))
                             .WithMessage("A Group Name already exists in this Group.");
                        break;
                }
            }
        }
    }
}