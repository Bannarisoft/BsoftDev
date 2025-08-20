using FluentValidation;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Common.Interfaces;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems; // DTOs
using Core.Application.Common.Text;

namespace InventoryManagement.API.Validation.Item.ItemDetail
{
    public sealed class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator(
            IItemCommandRepository itemRepo,
            IItemQueryRepository qryRepo,
            IMaxLengthProvider maxLenProvider,
            IValidator<ItemPurchaseDto> purchaseV,
            IValidator<ItemInventoryDto> inventoryV,
            IValidator<ItemQualityDto> qualityV,
            IValidator<ItemSupplierDto> supplierRowV,
            IValidator<ItemManufactureDto> manuRowV,
            IValidator<ItemUomDto> uomRowV)
        {
            CascadeMode = CascadeMode.Stop;

            // --- Safe max lengths (fallback to defaults if provider not ready) ---
            int codeMax = 50, nameMax = 200;
            try
            {
                codeMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(
                              nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemCode)) ?? 50;
                nameMax = maxLenProvider.GetMaxLength<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(
                              nameof(Core.Domain.Entities.Item.ItemDetail.ItemMaster.ItemName)) ?? 200;
            }
            catch { /* use defaults */ }

            // -------- Basic required & length rules --------
            RuleFor(x => x.Payload.Id)
                .GreaterThan(0).WithMessage("Item Id is required.");

            RuleFor(x => x.Payload.ItemName)
                .NotEmpty().WithMessage("ItemName is required.")
                .MaximumLength(nameMax);

            RuleFor(x => x.Payload.UnitId)
                .GreaterThan(0).WithMessage("UnitId is required.");

            // If you allow user-supplied ItemCode edits, keep this; otherwise you can remove it.
            RuleFor(x => x.Payload.ItemCode)
                .MaximumLength(codeMax)
                .When(x => !string.IsNullOrWhiteSpace(x.Payload.ItemCode));

            // -------- Uniqueness (UPDATE) --------
            // ItemCode unique excluding current record
            RuleFor(x => x.Payload.ItemCode)
                .MustAsync(async (cmd, code, ct) =>
                {
                    if (string.IsNullOrWhiteSpace(code)) return true;
                    try { return !await itemRepo.ExistsByCodeForUpdateAsync(code, cmd.Payload.Id, ct); }
                    catch { return true; } // don't hard-fail validation on infra issues
                })
                .WithMessage("Another item with the same ItemCode exists.")
                .When(x => !string.IsNullOrWhiteSpace(x.Payload.ItemCode));

            // ItemName “smart” unique excluding current record
            RuleFor(x => x.Payload.ItemName)
                .MustAsync(async (cmd, name, ct) =>
                {
                    try { return !await itemRepo.ExistsByNameSmartForUpdateAsync(name, cmd.Payload.Id, ct); }
                    catch { return true; }
                })
                .WithMessage("Item name already exists.");

            // “Too similar” check (fail-open if query repo not implemented)
            RuleFor(x => x.Payload.ItemName)
                .MustAsync(async (cmd, name, ct) =>
                {
                    try
                    {
                        var norm = NameSimilarity.Normalize(name);
                        var candidates = await qryRepo.GetCandidateItemNamesAsync(norm, 200, ct);
                        if (candidates is null || candidates.Count == 0) return true;
                        return !NameSimilarity.IsTooSimilarToAny(norm, candidates);
                    }
                    catch { return true; }
                })
                .WithMessage("This name is highly similar to an existing item. Please choose a more distinct name.");

            // -------- Optional tabs: validate only when they actually contain data --------
            When(x => x.Payload.Purchase is { } p && !IsEmptyPurchase(p),
                () => RuleFor(x => x.Payload.Purchase!).SetValidator(purchaseV));

            When(x => x.Payload.Inventory is { } inv && !IsEmptyInventory(inv),
                () => RuleFor(x => x.Payload.Inventory!).SetValidator(inventoryV));

            When(x => x.Payload.Quality is { } q && !IsEmptyQuality(q),
                () => RuleFor(x => x.Payload.Quality!).SetValidator(qualityV));

            // -------- Lists: skip blank rows, then apply row validators --------
            RuleForEach(x => x.Payload.Suppliers)
                .Where(r => !(r.SupplierId == 0 && r.UnitId == 0 && string.IsNullOrWhiteSpace(r.SupplierPartNo)))
                .SetValidator(supplierRowV);

            RuleForEach(x => x.Payload.Manufacture)
                .Where(r => !(r.UnitId == 0 && r.ManufacturingTypeId == 0))
                .SetValidator(manuRowV);

            RuleForEach(x => x.Payload.Uoms)
                .Where(r => r.ConversionUOMId.HasValue || r.ConversionRate.HasValue)
                .SetValidator(uomRowV);

            // -------- Duplicate prevention in lists --------
            RuleFor(x => x.Payload.Suppliers)
                .Must(list => list.Select(s => (s.SupplierId, s.UnitId)).Distinct().Count() == list.Count)
                .WithMessage("Duplicate Supplier+Unit rows are not allowed.");

            RuleFor(x => x.Payload.Manufacture)
                .Must(list => list.Select(m => (m.UnitId, m.ManufacturingTypeId)).Distinct().Count() == list.Count)
                .WithMessage("Duplicate Unit+ManufacturingType rows are not allowed.");

            RuleFor(x => x.Payload.Uoms)
                .Must(list =>
                {
                    var withId = list.Where(u => u.ConversionUOMId.HasValue).ToList();
                    return withId.Select(u => u.ConversionUOMId!.Value).Distinct().Count() == withId.Count;
                })
                .WithMessage("Duplicate ConversionUOM not allowed for the same Item.");
        }

        // -------- helpers to detect “empty” tabs (distinct names; used above) --------
        private static bool IsEmptyPurchase(ItemPurchaseDto p) =>
            !p.PurchaseUomId.HasValue && !p.LeadTimeDays.HasValue &&
            !p.SafetyStock.HasValue && !p.GrProcessingTimeDays.HasValue &&
            !p.OriginCountryId.HasValue && string.IsNullOrWhiteSpace(p.TariffNumber);

        private static bool IsEmptyInventory(ItemInventoryDto p) =>
            !p.Weight.HasValue && !p.WeightUomId.HasValue &&
            !p.DefaultMaterialRequestTypeId.HasValue && !p.ValuationMethodId.HasValue &&
            !p.ShelfLife.HasValue && !p.UpperTolerance.HasValue && !p.LowerTolerance.HasValue &&
            string.IsNullOrWhiteSpace(p.BatchNumberSeries) && string.IsNullOrWhiteSpace(p.SerialNumberSeries) &&
            !p.ReorderLevel.HasValue && !p.ReorderQty.HasValue && !p.RequestTypeId.HasValue &&
            !p.AllowNegativeStock && !p.BatchManagement && !p.ApplyBatchNumber;

        private static bool IsEmptyQuality(ItemQualityDto p) =>
            !p.InspectionTemplateId.HasValue && !p.CertificateTypeId.HasValue &&
            !p.InspLotProcessingTime.HasValue && !p.InspectionRequired &&
            !p.QualityInspectionFree && !p.IsCertificateRequiredFromSupplier;
    }
}
