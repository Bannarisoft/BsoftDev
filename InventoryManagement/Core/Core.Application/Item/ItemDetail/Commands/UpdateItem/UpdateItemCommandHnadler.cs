using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Commands.UpdateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems; // VariantValueDto
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;
using static Core.Domain.Common.BaseEntity; // for Status enum

namespace Core.Application.Item.ItemDetail.Commands.UpdateItem
{
    public sealed class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdateItemCommandHandler> _logger;

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
            IUnitOfWork uow, IMapper mapper, IMediator mediator, ILogger<UpdateItemCommandHandler> logger,
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
            _uow = uow; _mapper = mapper; _mediator = mediator; _logger = logger;
            _itemRepo = itemRepo; _purchaseRepo = purchaseRepo; _inventoryRepo = inventoryRepo; _qualityRepo = qualityRepo;
            _supplierRepo = supplierRepo; _manuRepo = manuRepo; _uomRepo = uomRepo;
            _variantCmd = variantCmd; _variantQry = variantQry;
        }

        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            try
            {
                var p = request.Payload;

                await _uow.ExecuteInTransactionAsync<Unit>(async _ =>
                {
                    // 1) Load + guard
                    var tpl = await _itemRepo.GetTrackingAsync(request.Payload.Id, ct)
                              ?? throw new KeyNotFoundException("Item not found.");

                    if (await _itemRepo.ExistsByCodeForUpdateAsync(p.ItemCode, tpl.Id, ct))
                        throw new InvalidOperationException("Another item with the same ItemCode exists.");

                    // 2) Base update (variants handled separately)
                    _mapper.Map(p, tpl);
                    await _itemRepo.UpdateAsync(tpl, ct);

                    // 3) Tabs upsert (safe; no key edits)
                    if (p.Purchase is not null)
                    {
                        var existingP = await _purchaseRepo.GetByItemIdAsync(tpl.Id, ct);
                        if (existingP is null)
                        {
                            var addP = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                            addP.ItemId = tpl.Id;
                            await _purchaseRepo.CreateAsync(addP, ct);
                        }
                        else
                        {
                            _mapper.Map(p.Purchase, existingP);   // keys ignored by profile
                            await _purchaseRepo.UpdateAsync(existingP, ct);
                        }
                    }

                    if (p.Inventory is not null)
                    {
                        var existingInv = await _inventoryRepo.GetByItemIdAsync(tpl.Id, ct);
                        if (existingInv is null)
                        {
                            var addInv = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                            addInv.ItemId = tpl.Id;
                            await _inventoryRepo.CreateAsync(addInv, ct);
                        }
                        else
                        {
                            _mapper.Map(p.Inventory, existingInv); // keys ignored by profile
                            await _inventoryRepo.UpdateAsync(existingInv, ct);
                        }
                    }

                    if (p.Quality is not null)
                    {
                        var existingQ = await _qualityRepo.GetByItemIdAsync(tpl.Id, ct);
                        if (existingQ is null)
                        {
                            var addQ = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                            addQ.ItemId = tpl.Id;
                            await _qualityRepo.CreateAsync(addQ, ct);
                        }
                        else
                        {
                            _mapper.Map(p.Quality, existingQ); // keys ignored by profile
                            await _qualityRepo.UpdateAsync(existingQ, ct);
                        }
                    }

                    // 4) Collections
                    if (p.Suppliers is not null)
                    {
                        await _supplierRepo.UpdateAsync(tpl.Id, p.Suppliers, ct);
                    }
                    if (p.Manufacture is not null)
                    {
                        await _manuRepo.UpdateAsync(tpl.Id, p.Manufacture, ct);
                    }

                    // UOMs (collection)
                    if (p.Uoms is not null)
                    {
                        await _uomRepo.UpdateAsync(tpl.Id, p.Uoms, ct);
                    }

                    // 5) Variants — ONLY ADD (do not touch existing values or children)
                    if (p.HasVariants)
                    {
                        // 5.1 Load current allowed values from DB
                        var currentAllowed = await _variantQry.GetForItemGroupedAsync(tpl.Id, ct); // Dict<int, List<string>>

                        // Normalize helper
                        static string N(string s) => (s ?? string.Empty).Trim();

                        // Case-insensitive lookup of existing options per attribute
                        var curLookup = currentAllowed.ToDictionary(
                            kv => kv.Key,
                            kv => new HashSet<string>(kv.Value.Select(v => N(v)), StringComparer.OrdinalIgnoreCase));

                        // 5.2 Identify ONLY NEW template values present in payload
                        var newTemplateValues = new List<VariantValueDto>();
                        var newOptionsByAttr = new Dictionary<int, HashSet<string>>();
                        if (p.VariantValues is { Count: > 0 })
                        {
                            foreach (var v in p.VariantValues)
                            {
                                if (v is null || string.IsNullOrWhiteSpace(v.OptionValue)) continue;
                                var attrId = v.AttributeId;
                                var opt    = N(v.OptionValue);

                                if (!curLookup.TryGetValue(attrId, out var existingSet) || !existingSet.Contains(opt))
                                {
                                    newTemplateValues.Add(new VariantValueDto
                                    {
                                        AttributeId = attrId,
                                        OptionValue = opt,
                                        VariantBasedOn = v.VariantBasedOn,
                                        AttributeGroupId= v.AttributeGroupId
                                    });

                                    if (!newOptionsByAttr.TryGetValue(attrId, out var set))
                                        newOptionsByAttr[attrId] = set = new(StringComparer.OrdinalIgnoreCase);
                                    set.Add(opt);
                                }
                            }
                        }

                        // 5.3 INSERT-ONLY new template values (never delete/update existing)
                        if (newTemplateValues.Count > 0)
                        {
                            // NOTE: repository must be insert-only (no replace). See IItemVariantValueCommandRepository
                            await _variantCmd.AddMissingTemplateOptionsAsync(tpl.Id, newTemplateValues, ct);

                            // Merge into currentAllowed & curLookup for next step
                            foreach (var v in newTemplateValues)
                            {
                                if (!currentAllowed.TryGetValue(v.AttributeId, out var list))
                                {
                                    list = new List<string>();
                                    currentAllowed[v.AttributeId] = list;
                                }
                                if (!list.Any(s => string.Equals(s, v.OptionValue, StringComparison.OrdinalIgnoreCase)))
                                    list.Add(v.OptionValue);
                            }
                            foreach (var (aid, set) in newOptionsByAttr)
                            {
                                if (!curLookup.TryGetValue(aid, out var exist))
                                    curLookup[aid] = new HashSet<string>(set, StringComparer.OrdinalIgnoreCase);
                                else
                                    foreach (var val in set) exist.Add(val);
                            }
                        }

                        // If no NEW values, stop here — don’t touch children.
                        if (newOptionsByAttr.Count == 0)
                            return Unit.Value;

                        // 5.4 Generate all combos, then keep only those that include at least one NEW option
                        if (currentAllowed.Count > 0 && currentAllowed.All(kv => kv.Value.Count > 0))
                        {
                            var allCombos = Cartesian(currentAllowed);

                            bool UsesAnyNewOption(Dictionary<int, string> combo)
                                => combo.Any(kv => newOptionsByAttr.TryGetValue(kv.Key, out var set) && set.Contains(N(kv.Value)));

                            var candidateCombos = allCombos.Where(UsesAnyNewOption).ToList();
                            var candidateKeys   = new HashSet<string>(candidateCombos.Select(ComboKey), StringComparer.Ordinal);

                            // Existing child combos via mapping
                            var mapped = await _variantQry.GetExistingChildCombosWithIdsAsync(tpl.Id, ct);
                            var existingKeys = new HashSet<string>(mapped.Keys, StringComparer.Ordinal);

                            // Fallback: infer from child names (in case mapping wasn’t set in the past)
                            await AddExistingKeysFromChildNames(existingKeys, tpl, currentAllowed, ct);

                            // Only create truly new children
                            var toAddKeys = candidateKeys.Except(existingKeys).ToList();

                            var seq = 1;
                            foreach (var key in toAddKeys)
                            {
                                var combo = ParseKey(key);
                                var childName = BuildChildName(tpl.ItemName, combo);

                                var gen = await NextChildCodeAsync(tpl.ItemCode, seq, ct);
                                var childCode = gen.Code;
                                seq = gen.NextSeq;

                                var child = new Core.Domain.Entities.Item.ItemDetail.ItemMaster
                                {
                                    UnitId                 = tpl.UnitId,
                                    ItemCode               = childCode,
                                    ItemName               = childName,
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

                                // Optionally clone tabs/collections
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

                                // Link template options -> child (helps future detection)
                                foreach (var kv in combo)
                                {
                                    await _variantCmd.MapOptionToChildAsync(
                                        templateItemId: tpl.Id,
                                        attributeId: kv.Key,
                                        optionValue: kv.Value,
                                        childItemId: childId,
                                        ct: ct);
                                }
                            }
                        }
                    }

                    return Unit.Value; // commit
                }, ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                    "Update",
                    request.Payload.Id.ToString(),
                    request.Payload.ItemName,
                    "Item updated; existing variants untouched; only new variants/children inserted.",
                    "ItemMaster"), ct);

                return Unit.Value;
            }
            catch (NotImplementedException nie)
            {
                _logger.LogError(nie, "NotImplemented in UpdateItemCommandHandler");
                throw;
            }
        }

        // ------- helpers -------
        private async Task AddExistingKeysFromChildNames(
            HashSet<string> keys,
            Core.Domain.Entities.Item.ItemDetail.ItemMaster template,
            Dictionary<int, List<string>> allowed,
            CancellationToken ct)
        {
            var orderedAttrIds = allowed.Keys.OrderBy(k => k).ToList();
            var childIds = await _itemRepo.GetChildIdsAsync(template.Id, ct);

            foreach (var cid in childIds)
            {
                var child = await _itemRepo.GetTrackingAsync(cid, ct);
                if (child?.ItemName is null) continue;

                var name = child.ItemName;
                var suffix = name.StartsWith(template.ItemName, StringComparison.OrdinalIgnoreCase)
                    ? name.Substring(template.ItemName.Length).Trim()
                    : name.Trim();

                if (string.IsNullOrWhiteSpace(suffix)) continue;

                string[] tokens =
                    suffix.Contains('/')
                    ? suffix.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    : suffix.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length != orderedAttrIds.Count) continue;

                var combo = new Dictionary<int, string>();
                for (int i = 0; i < orderedAttrIds.Count; i++)
                    combo[orderedAttrIds[i]] = tokens[i];

                keys.Add(ComboKey(combo));
            }
        }

        private async Task<(string Code, int NextSeq)> NextChildCodeAsync(string baseCode, int seq, CancellationToken ct)
        {
            // Generate base-000 style codes and ensure uniqueness
            while (true)
            {
                var code = $"{baseCode}-{seq:000}";
                if (!await _itemRepo.ExistsByCodeForCreateAsync(code, ct))
                    return (code, seq + 1);

                seq++; // try next
            }
        }

        private static List<Dictionary<int, string>> Cartesian(Dictionary<int, List<string>> src)
        {
            var result = new List<Dictionary<int, string>> { new() };
            foreach (var kv in src)
            {
                var next = new List<Dictionary<int, string>>();
                foreach (var partial in result)
                foreach (var val in kv.Value)
                    next.Add(new Dictionary<int, string>(partial) { [kv.Key] = val });
                result = next;
            }
            return result;
        }

        private static string BuildChildName(string baseName, Dictionary<int, string> combo)
        {
            var label = string.Join(" / ", combo.Values);
            return $"{baseName} {label}".Trim();
        }

        private static string ComboKey(Dictionary<int, string> combo)
        {
            var sb = new StringBuilder();
            foreach (var kv in combo.OrderBy(k => k.Key))
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append(kv.Key).Append(':').Append(kv.Value.Trim().ToLowerInvariant());
            }
            return sb.ToString();
        }

        private static Dictionary<int, string> ParseKey(string key)
        {
            var dict = new Dictionary<int, string>();
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
