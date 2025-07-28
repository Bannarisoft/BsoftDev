using Core.Application.Common.Interfaces.Item.ItemGroup;
using Core.Application.Item.ItemGroup.Commands.UpdateItemGroup;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemGroup
{
    public class UpdateItemGroupCommandValidator : AbstractValidator<UpdateItemGroupCommand>
    {        
        private readonly List<ValidationRule> _validationRules;                    
        private readonly IItemGroupCommandRepository _itemGroupCommandRepository;
        private readonly IItemGroupQueryRepository _itemGroupQueryRepository;  
        public UpdateItemGroupCommandValidator(MaxLengthProvider maxLengthProvider, IItemGroupCommandRepository itemGroupCommandRepository,IItemGroupQueryRepository itemGroupQueryRepository)
        {
            _itemGroupCommandRepository = itemGroupCommandRepository;            
            _itemGroupQueryRepository = itemGroupQueryRepository;
            var maxLength = maxLengthProvider.GetMaxLength<Core.Domain.Entities.Item.ItemGroup>("ItemGroupName") ?? 250;

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
                        RuleFor(x => x.ItemGroupName)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateItemGroupCommand.ItemGroupName)} {rule.Error}");
                        RuleFor(x => x.ItemGroupCode)
                            .NotEmpty()
                            .WithMessage($"{nameof(UpdateItemGroupCommand.ItemGroupCode)} {rule.Error}");
                        break;
                    case "MaxLength":
                         RuleFor(x => x.ItemGroupName)
                            .MaximumLength(maxLength)
                            .WithMessage($"{nameof(UpdateItemGroupCommand.ItemGroupName)} {rule.Error}");
                        break;
                    case "AlreadyExists":
                          RuleFor(x => x.ItemGroupName)
                           .NotEmpty()
                           .WithMessage($"{nameof(UpdateItemGroupCommand.ItemGroupName)} {rule.Error}")
                           .MustAsync(async (command, moduleName, cancellation) =>
                            !await _itemGroupCommandRepository.IsNameDuplicateAsync(moduleName,command.Id))
                             .WithMessage("A Group Name already exists in this Group.");
                        break;
                    case "RecordNotFound":
                        RuleFor(x => x.Id)
                            .MustAsync(async (Id, cancellation) =>
                                await _itemGroupQueryRepository.NotFoundAsync(Id))
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