using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems; // VariantValueDto
using Core.Domain.Events;
using MediatR;
using System.Text;
using static Core.Domain.Common.BaseEntity; // for Status enum

namespace Core.Application.Item.ItemDetail.Commands.UpdateItem
{
    public sealed class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private readonly IItemCommandRepository _itemRepo;
        private readonly IItemPurchaseCommandRepository _purchaseRepo;
        private readonly IItemInventoryCommandRepository _inventoryRepo;
        private readonly IItemQualityCommandRepository _qualityRepo;
        private readonly IItemSupplierCommandRepository _supplierRepo;
        private readonly IItemManufactureCommandRepository _manuRepo;
        private readonly IItemUomCommandRepository _uomRepo;

        private readonly IItemVariantValueCommandRepository _variantCmd;
        private readonly IItemVariantValueQueryRepository _variantQry;

        public UpdateItemCommandHandler(
            IUnitOfWork uow, IMapper mapper, IMediator mediator,
            IItemCommandRepository itemRepo,
            IItemPurchaseCommandRepository purchaseRepo,
            IItemInventoryCommandRepository inventoryRepo,
            IItemQualityCommandRepository qualityRepo,
            IItemSupplierCommandRepository supplierRepo,
            IItemManufactureCommandRepository manuRepo,
            IItemUomCommandRepository uomRepo,
            IItemVariantValueCommandRepository variantCmd,
            IItemVariantValueQueryRepository variantQry)
        {
            _uow = uow; _mapper = mapper; _mediator = mediator;
            _itemRepo = itemRepo; _purchaseRepo = purchaseRepo; _inventoryRepo = inventoryRepo; _qualityRepo = qualityRepo;
            _supplierRepo = supplierRepo; _manuRepo = manuRepo; _uomRepo = uomRepo;
            _variantCmd = variantCmd; _variantQry = variantQry;
        }

        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var p = request.Payload;

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // 1) Load + guard
                var tpl = await _itemRepo.GetTrackingAsync(request.Id, ct)
                          ?? throw new KeyNotFoundException("Item not found.");

                if (await _itemRepo.ExistsByCodeForUpdateAsync(p.ItemCode, request.Id, ct))
                    throw new InvalidOperationException("Another item with the same ItemCode exists.");

                var oldTemplateName = tpl.ItemName;

                // 2) Map base
                _mapper.Map(p, tpl);
                await _itemRepo.UpdateAsync(tpl, ct);

                // 3) Tabs upsert
                if (p.Purchase is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                    e.ItemId = tpl.Id; await _purchaseRepo.UpdateAsync(e, ct);
                }
                if (p.Inventory is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                    e.ItemId = tpl.Id; await _inventoryRepo.UpdateAsync(e, ct);
                }
                if (p.Quality is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                    e.ItemId = tpl.Id; await _qualityRepo.UpdateAsync(e, ct);
                }

                // 4) Collections sync
                await _supplierRepo.UpdateAsync(tpl.Id, p.Suppliers ?? new(), ct);
                await _manuRepo.UpdateAsync(tpl.Id, p.Manufacture ?? new(), ct);
                await _uomRepo.UpdateAsync(tpl.Id, p.Uoms ?? new(), ct);

                // 5) Variants (NO DELETE logic)
                if (p.HasVariants)
                {
                    // 5.1 Update template's allowed set
                    await _variantCmd.UpsertListAsync(tpl.Id, p.VariantValues ?? new(), ct);

                    // 5.2 Desired combos from template
                    var allowed = await _variantQry.GetForItemGroupedAsync(tpl.Id, ct);
                    if (allowed.Count > 0 && allowed.All(kv => kv.Value.Count > 0))
                    {
                        var desiredCombos = Cartesian(allowed); // List<Dictionary<int,string>>
                        var desiredKeys   = new HashSet<string>(desiredCombos.Select(ComboKey));

                        // Existing children (key -> childId)
                        var existing = await _variantQry.GetExistingChildCombosWithIdsAsync(tpl.Id, ct);
                        var existingKeys = existing.Keys.ToHashSet(StringComparer.Ordinal);

                        var toAdd  = desiredKeys.Except(existingKeys).ToList();
                        var toKeep = desiredKeys.Intersect(existingKeys).ToList();
                        var toObsolete = existingKeys.Except(desiredKeys).ToList(); // will be deactivated, not deleted

                        // 5.3 Add missing children
                        foreach (var key in toAdd)
                        {
                            var combo = ParseKey(key);
                            var childName = BuildChildName(tpl.ItemName, combo);

                            var child = new Core.Domain.Entities.Item.ItemDetail.ItemMaster
                            {
                                UnitId                 = tpl.UnitId,
                                ItemCode               = tpl.ItemCode, // ensure uniqueness externally if needed
                                ItemName               = childName,    // ONLY change
                                HSNId                  = tpl.HSNId,
                                ItemGroupId            = tpl.ItemGroupId,
                                ItemCategoryId         = tpl.ItemCategoryId,
                                StockUomId             = tpl.StockUomId,
                                ItemClassificationId   = tpl.ItemClassificationId,
                                Description            = tpl.Description,
                                ValidFrom              = tpl.ValidFrom,
                                XPlantMaterialStatusId = tpl.XPlantMaterialStatusId,
                                IsStockItem            = tpl.IsStockItem,
                                MaintainStock          = tpl.MaintainStock,
                                HasVariants            = false,
                                ParentItemId           = tpl.Id,
                                IsActive               = Status.Active,
                                IsDeleted              = tpl.IsDeleted
                            };
                            var childId = await _itemRepo.CreateAsync(child, ct);

                            // clone tabs/collections from latest payload
                            if (p.Purchase is not null)
                            {
                                var cp = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                                cp.ItemId = childId; await _purchaseRepo.CreateAsync(cp, ct);
                            }
                            if (p.Inventory is not null)
                            {
                                var ci = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                                ci.ItemId = childId; await _inventoryRepo.CreateAsync(ci, ct);
                            }
                            if (p.Quality is not null)
                            {
                                var cq = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                                cq.ItemId = childId; await _qualityRepo.CreateAsync(cq, ct);
                            }
                            if (p.Suppliers is { Count: > 0 })  await _supplierRepo.UpdateAsync(childId, p.Suppliers, ct);
                            if (p.Manufacture is { Count: > 0 }) await _manuRepo.UpdateAsync(childId, p.Manufacture, ct);
                            if (p.Uoms is { Count: > 0 })       await _uomRepo.UpdateAsync(childId, p.Uoms, ct);

                            // set child’s selected values
                            var selected = combo.Select(kv => new VariantValueDto { AttributeId = kv.Key, OptionValue = kv.Value }).ToList();
                            await _variantCmd.UpsertListAsync(childId, selected, ct);
                        }

                        // 5.4 Keep existing children: if template name changed -> rename to keep pattern
                        if (!string.Equals(oldTemplateName, tpl.ItemName, StringComparison.Ordinal))
                        {
                            foreach (var key in toKeep)
                            {
                                var childId = existing[key];
                                var combo = ParseKey(key);
                                var newName = BuildChildName(tpl.ItemName, combo);

                                var child = await _itemRepo.GetTrackingAsync(childId, ct);
                                if (child is not null)
                                {
                                    child.ItemName = newName;
                                    child.IsActive = Status.Active; // ensure active
                                    await _itemRepo.UpdateAsync(child, ct);
                                }
                            }
                        }

                        // 5.5 Obsolete children: **deactivate** (NO delete)
                        foreach (var key in toObsolete)
                        {
                            var childId = existing[key];
                            var child = await _itemRepo.GetTrackingAsync(childId, ct);
                            if (child is not null)
                            {
                                child.IsActive = Status.Inactive;
                                await _itemRepo.UpdateAsync(child, ct);
                            }
                        }
                    }
                    else
                    {
                        // No allowed combos -> deactivate all children (no delete)
                        var childIds = await _itemRepo.GetChildIdsAsync(tpl.Id, ct);
                        foreach (var cid in childIds)
                        {
                            var child = await _itemRepo.GetTrackingAsync(cid, ct);
                            if (child is not null)
                            {
                                child.IsActive = Status.Inactive;
                                await _itemRepo.UpdateAsync(child, ct);
                            }
                        }
                    }
                }
                else
                {
                    // Variants disabled: do NOT delete anything. Optionally deactivate children.
                    var childIds = await _itemRepo.GetChildIdsAsync(tpl.Id, ct);
                    foreach (var cid in childIds)
                    {
                        var child = await _itemRepo.GetTrackingAsync(cid, ct);
                        if (child is not null)
                        {
                            child.IsActive = Status.Inactive;
                            await _itemRepo.UpdateAsync(child, ct);
                        }
                    }
                    // Optionally keep or clear template VariantValues; no delete either way.
                    // await _variantCmd.ClearForItemAsync(tpl.Id, ct); // <-- skip per "no delete"
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                    "Update",
                    tpl.Id.ToString(),
                    tpl.ItemName,
                    "Item updated; children added/renamed; obsolete children deactivated (no deletes).",
                    "ItemMaster"), ct);

                return Unit.Value;
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        // --------- helpers ----------
        private static List<Dictionary<int, string>> Cartesian(Dictionary<int, List<string>> src)
        {
            var result = new List<Dictionary<int, string>> { new() };
            foreach (var kv in src)
            {
                var next = new List<Dictionary<int, string>>();
                foreach (var partial in result)
                foreach (var val in kv.Value)
                {
                    var copy = new Dictionary<int, string>(partial) { [kv.Key] = val };
                    next.Add(copy);
                }
                result = next;
            }
            return result;
        }

        private static string BuildChildName(string baseName, Dictionary<int, string> combo)
        {
            var label = string.Join(" / ", combo.Values);
            return $"{baseName} {label}".Trim();
        }

        private static string ComboKey(Dictionary<int,string> combo)
        {
            var sb = new StringBuilder();
            foreach (var kv in combo.OrderBy(k => k.Key))
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append(kv.Key).Append(':').Append(kv.Value.Trim().ToLowerInvariant());
            }
            return sb.ToString();
        }

        private static Dictionary<int,string> ParseKey(string key)
        {
            // key format: "10:red|11:s"
            var dict = new Dictionary<int,string>();
            if (string.IsNullOrWhiteSpace(key)) return dict;
            var pairs = key.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in pairs)
            {
                var parts = p.Split(':', 2);
                if (parts.Length == 2 && int.TryParse(parts[0], out var aid))
                    dict[aid] = parts[1];
            }
            return dict;
        }
    }
}
