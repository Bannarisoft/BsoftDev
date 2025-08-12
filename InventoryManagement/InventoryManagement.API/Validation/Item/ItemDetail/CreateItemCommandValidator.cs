using FluentValidation;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Common.Interfaces;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemCommandValidator(
            IItemCommandRepository itemRepo,
            IMaxLengthProvider maxLenProvider,
            ItemPurchaseDtoValidator purchaseV,
            ItemInventoryDtoValidator inventoryV,
            ItemQualityDtoValidator qualityV,
            ItemSupplierDtoValidator supplierRowV,
            ItemManufacturingDtoValidator manuRowV,
            ItemUomDtoValidator uomRowV // optional
        )
        {
            var rules = ValidationRuleLoader.LoadValidationRules();
            if (rules == null || !rules.Any())
                throw new ArgumentException("Validation rules could not be loaded.");

            // DB max lengths (fallbacks used if provider returns null)
            var codeMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemCode)) ?? 50;
            var nameMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemName)) ?? 200;
            var hsnMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.HSNCode)) ?? 20;

            foreach (var rule in rules)
            {
                switch (rule.Rule)
                {
                    case "NotEmpty":
                        RuleFor(x => x.Payload.ItemCode).NotEmpty().WithMessage($"{nameof(CreateItemCommand.Payload.ItemCode)} {rule.Error}");
                        RuleFor(x => x.Payload.ItemName).NotEmpty().WithMessage($"{nameof(CreateItemCommand.Payload.ItemName)} {rule.Error}");
                        RuleFor(x => x.Payload.UnitId).GreaterThan(0).WithMessage($"{nameof(CreateItemCommand.Payload.UnitId)} {rule.Error}");
                        break;

                    case "MaxLength":
                        RuleFor(x => x.Payload.ItemCode).MaximumLength(codeMax).WithMessage($"{nameof(CreateItemCommand.Payload.ItemCode)} {rule.Error}");
                        RuleFor(x => x.Payload.ItemName).MaximumLength(nameMax).WithMessage($"{nameof(CreateItemCommand.Payload.ItemName)} {rule.Error}");
                        RuleFor(x => x.Payload.HSNCode).MaximumLength(hsnMax).When(x => !string.IsNullOrWhiteSpace(x.Payload.HSNCode))
                            .WithMessage($"{nameof(CreateItemCommand.Payload.HSNCode)} {rule.Error}");
                        break;

                    case "AlreadyExists":
                        RuleFor(x => x.Payload.ItemCode)
                            .MustAsync(async (cmd, code, ct) => !(await itemRepo.ExistsByCodeForCreateAsync(code, ct)))
                            .WithMessage("ItemCode already exists.");
                        break;
                }
            }

            // Tabs
            When(x => x.Payload.Purchase is not null, () => RuleFor(x => x.Payload.Purchase!).SetValidator(purchaseV));
            When(x => x.Payload.Inventory is not null, () => RuleFor(x => x.Payload.Inventory!).SetValidator(inventoryV));
            When(x => x.Payload.Quality is not null, () => RuleFor(x => x.Payload.Quality!).SetValidator(qualityV));

            // Lists
            RuleForEach(x => x.Payload.Suppliers).SetValidator(supplierRowV);
            RuleForEach(x => x.Payload.Manufacture).SetValidator(manuRowV);

            // Duplicate prevention in lists
            RuleFor(x => x.Payload.Suppliers)
                .Must(list => list.Select(s => (s.SupplierId, s.UnitId)).Distinct().Count() == list.Count)
                .WithMessage("Duplicate Supplier+Unit rows are not allowed.");

            RuleFor(x => x.Payload.Manufacture)
                .Must(list => list.Select(m => (m.UnitId, m.ManufacturingTypeId)).Distinct().Count() == list.Count)
                .WithMessage("Duplicate Unit+ManufacturingType rows are not allowed.");

            RuleFor(x => x.Payload.Uoms)
            .Must(list => list
                .Where(u => u.ConversionUOMId.HasValue)
                .Select(u => u.ConversionUOMId!.Value)
                .Distinct().Count() == list.Count)
            .WithMessage("Duplicate ConversionUOM not allowed for the same Item.");
        }
    }
}
