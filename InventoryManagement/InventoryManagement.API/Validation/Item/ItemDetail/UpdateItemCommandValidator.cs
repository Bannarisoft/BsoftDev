
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using FluentValidation;
using InventoryManagement.API.Validation.Common;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator(
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
            Include(new CreateLikeRules(itemRepo, maxLenProvider, purchaseV, inventoryV, qualityV, supplierRowV, manuRowV, uomRowV));

            // Unique ItemCode excluding current Id
            RuleFor(x => x)
                .MustAsync(async (cmd, ct) => !(await itemRepo.ExistsByCodeForUpdateAsync(cmd.Payload.ItemCode, cmd.Id, ct)))
                .WithMessage("Another item with the same ItemCode exists.");
        }

        private sealed class CreateLikeRules : AbstractValidator<UpdateItemCommand>
        {
            public CreateLikeRules(
                IItemCommandRepository itemRepo,
                IMaxLengthProvider maxLenProvider,
                ItemPurchaseDtoValidator purchaseV,
                ItemInventoryDtoValidator inventoryV,
                ItemQualityDtoValidator qualityV,
                ItemSupplierDtoValidator supplierRowV,
                ItemManufacturingDtoValidator manuRowV,
                ItemUomDtoValidator uomRowV)
            {
                var rules = ValidationRuleLoader.LoadValidationRules();

                var codeMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemCode)) ?? 50;
                var nameMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemName)) ?? 200;
                var hsnMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.HSNCode)) ?? 20;

                foreach (var rule in rules)
                {
                    switch (rule.Rule)
                    {
                        case "NotEmpty":
                            RuleFor(x => x.Payload.ItemCode).NotEmpty();
                            RuleFor(x => x.Payload.ItemName).NotEmpty();
                            RuleFor(x => x.Payload.UnitId).GreaterThan(0);
                            break;
                        case "MaxLength":
                            RuleFor(x => x.Payload.ItemCode).MaximumLength(codeMax);
                            RuleFor(x => x.Payload.ItemName).MaximumLength(nameMax);
                            RuleFor(x => x.Payload.HSNCode).MaximumLength(hsnMax).When(x => !string.IsNullOrWhiteSpace(x.Payload.HSNCode));
                            break;
                    }
                }

                // Tabs
                When(x => x.Payload.Purchase is not null, () => RuleFor(x => x.Payload.Purchase!).SetValidator(purchaseV));
                When(x => x.Payload.Inventory is not null, () => RuleFor(x => x.Payload.Inventory!).SetValidator(inventoryV));
                When(x => x.Payload.Quality is not null, () => RuleFor(x => x.Payload.Quality!).SetValidator(qualityV));

                // Lists + dups
                RuleForEach(x => x.Payload.Suppliers).SetValidator(supplierRowV);
                RuleForEach(x => x.Payload.Manufacture).SetValidator(manuRowV);

                RuleFor(x => x.Payload.Suppliers)
                    .Must(list => list.Select(s => (s.SupplierId, s.UnitId)).Distinct().Count() == list.Count)
                    .WithMessage("Duplicate Supplier+Unit rows are not allowed.");

                RuleFor(x => x.Payload.Manufacture)
                    .Must(list => list.Select(m => (m.UnitId, m.ManufacturingTypeId)).Distinct().Count() == list.Count)
                    .WithMessage("Duplicate Unit+ManufacturingType rows are not allowed.");
            }
        }
    }
}