using System.Text;
using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Common;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Item.ItemAggregate.Handlers
{
    public sealed class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateItemCommandHandler> _logger;
        private readonly IItemCommandRepository _itemRepo;
        private readonly IItemPurchaseCommandRepository _purchaseRepo;
        private readonly IItemInventoryCommandRepository _inventoryRepo;
        private readonly IItemQualityCommandRepository _qualityRepo;
        private readonly IItemSupplierCommandRepository _supplierRepo;
        private readonly IItemManufactureCommandRepository _manufactureRepo;
        private readonly IItemUomCommandRepository _uomRepo;
        private readonly IItemQueryRepository _itemQueryRepository;
        private readonly IItemVariantValueCommandRepository _variantCmd;
        private readonly IItemVariantValueQueryRepository _variantQry;

        public CreateItemCommandHandler(
            IUnitOfWork uow,
            IMapper mapper,
            IMediator mediator,
            IItemCommandRepository itemRepo,
            IItemPurchaseCommandRepository purchaseRepo,
            IItemInventoryCommandRepository inventoryRepo,
            IItemQualityCommandRepository qualityRepo,
            IItemSupplierCommandRepository supplierRepo,
            IItemManufactureCommandRepository manufactureRepo,
            IItemUomCommandRepository uomRepo,
            IItemQueryRepository itemQueryRepository,
            ILogger<CreateItemCommandHandler> logger,
            IItemVariantValueCommandRepository variantCmd,
            IItemVariantValueQueryRepository variantQry)
        {
            _uow = uow;
            _mapper = mapper;
            _mediator = mediator;
            _itemRepo = itemRepo;
            _purchaseRepo = purchaseRepo;
            _inventoryRepo = inventoryRepo;
            _qualityRepo = qualityRepo;
            _supplierRepo = supplierRepo;
            _manufactureRepo = manufactureRepo;
            _uomRepo = uomRepo;
            _itemQueryRepository = itemQueryRepository;
            _logger = logger;
            _variantCmd = variantCmd;
            _variantQry = variantQry;
        }

        public async Task<int> Handle(CreateItemCommand request, CancellationToken ct)
        {
            try
            {
                var p = request.Payload;

                if (!p.ItemGroupId.HasValue || !p.ItemCategoryId.HasValue)
                    throw new InvalidOperationException("ItemGroupId and ItemCategoryId are required to generate ItemCode.");

                var suggestedCode = await _itemQueryRepository.GetLatestItemCode(p.ItemGroupId.Value, p.ItemCategoryId.Value, ct);
                string? finalItemCode = null;
                var itemId = await _uow.ExecuteInTransactionAsync<int>(async _ =>
                {
                    var itemCode = suggestedCode ?? throw new InvalidOperationException("Failed to generate ItemCode.");
                    if (await _itemRepo.ExistsByCodeForCreateAsync(itemCode, ct))
                        throw new InvalidOperationException($"Generated ItemCode '{itemCode}' already exists.");
                    finalItemCode = itemCode; 
                    // 1) base
                    var item = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(p);
                    item.ItemCode = itemCode;
                    item.IsActive = BaseEntity.Status.Active;
                    var newId = await _itemRepo.CreateAsync(item, ct);

                    // 2) tabs
                    if (p.Purchase is not null)
                    {
                        var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                        e.ItemId = newId; await _purchaseRepo.CreateAsync(e, ct);
                    }
                    if (p.Inventory is not null)
                    {
                        var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                        e.ItemId = newId; await _inventoryRepo.CreateAsync(e, ct);
                    }
                    if (p.Quality is not null)
                    {
                        var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                        e.ItemId = newId; await _qualityRepo.CreateAsync(e, ct);
                    }

                    // 3) collections
                    if (p.Suppliers.Count > 0) await _supplierRepo.UpdateAsync(newId, p.Suppliers, ct);
                    if (p.Manufacture.Count > 0) await _manufactureRepo.UpdateAsync(newId, p.Manufacture, ct);
                    if (p.Uoms.Count > 0) await _uomRepo.UpdateAsync(newId, p.Uoms, ct);

                    // 4) template variant values (persist payload as the template)
                    // make sure your UpsertListAsync writes VariantBasedOn to the entity (see repo fix below)
                    Dictionary<int, int> basedOnByAttrId = new();
                    if (p.VariantValues is { Count: > 0 })
                    {
                        await _variantCmd.UpsertListAsync(newId, p.VariantValues, ct);

                        // Build maps from the *same* source to guarantee keys exist
                        var payloadVals = p.VariantValues
                            .Where(v => v != null && !string.IsNullOrWhiteSpace(v.OptionValue))
                            .Select(v => new VariantValueDto
                            {
                                AttributeId = v.AttributeId,
                                OptionValue = v.OptionValue.Trim(),
                                VariantBasedOn = v.VariantBasedOn,
                                AttributeGroupId=v.AttributeGroupId
                            })
                            .ToList();

                    }

                    // 5) auto-generate children if HasVariants and we have payload values                
                    if (p.HasVariants)
                    {
                        // Allowed values come from template payload
                        var allowed = p.VariantValues
                            .GroupBy(v => v.AttributeId)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(v => v.OptionValue.Trim())
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .ToList()
                            );

                        if (allowed.Count > 0 && allowed.All(kv => kv.Value.Count > 0))
                        {
                            var combos = Cartesian(allowed);
                            var existingComboKeys = await _variantQry.GetExistingChildComboKeysAsync(newId, ct);

                            int seq = 1;

                            foreach (var combo in combos)
                            {
                                var key = ComboKey(combo);
                                if (existingComboKeys.Contains(key)) continue;

                                var childName = BuildChildName(p.ItemName, combo);

                                // create the child
                                var child = new Core.Domain.Entities.Item.ItemDetail.ItemMaster
                                {
                                    UnitId = item.UnitId,
                                    ItemCode = $"{item.ItemCode}-{seq++:000}", // unique
                                    ItemName = childName,
                                    HSNId = item.HSNId,
                                    ItemGroupId = item.ItemGroupId,
                                    ItemCategoryId = item.ItemCategoryId,
                                    StockUomId = item.StockUomId,
                                    ItemClassificationId = item.ItemClassificationId,
                                    Description = item.Description,
                                    ValidFrom = item.ValidFrom,
                                    XPlantMaterialStatusId = item.XPlantMaterialStatusId,
                                    IsStockItem = item.IsStockItem,
                                    MaintainStock = item.MaintainStock,
                                    HasVariants = false,
                                    ParentItemId = newId,
                                    IsActive = BaseEntity.Status.Active ,
                                    IsDeleted = item.IsDeleted
                                };

                                var childId = await _itemRepo.CreateAsync(child, ct);

                                // clone tabs & collections (unchanged)
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
                                if (p.Suppliers is { Count: > 0 }) await _supplierRepo.UpdateAsync(childId, p.Suppliers, ct);
                                if (p.Manufacture is { Count: > 0 }) await _manufactureRepo.UpdateAsync(childId, p.Manufacture, ct);
                                if (p.Uoms is { Count: > 0 }) await _uomRepo.UpdateAsync(childId, p.Uoms, ct);

                                // IMPORTANT: instead of inserting child variant rows,
                                // link each template option to this child via NewItemId.
                                // NOTE: if you have multiple attributes, each attribute-option will point to the SAME child.
                                foreach (var kv in combo)
                                {
                                    await _variantCmd.MapOptionToChildAsync(
                                        templateItemId: newId,
                                        attributeId: kv.Key,
                                        optionValue: kv.Value,
                                        childItemId: childId,
                                        ct: ct);
                                }
                            }
                        }
                    }
                    return newId; // transaction wrapper commits
                }, ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                            "Create",
                            itemId.ToString(),
                            p.ItemName,
                            p.HasVariants ? "Template + variant children created automatically." : "Single item created (no variants).",
                            "ItemMaster"), ct);

                // post-commit image move (unchanged)
                if (itemId > 0 && !string.IsNullOrWhiteSpace(p.ItemImage))
                {
                    try
                    {
                        var baseDirectory = await _itemQueryRepository.GetBaseDirectoryAsync(ct);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);

                        var tempFileName = p.ItemImage;
                        var tempFullPath = Path.Combine(uploadPath, tempFileName);
                        EnsureDirectoryExists(Path.GetDirectoryName(tempFullPath)!);

                        if (File.Exists(tempFullPath))
                        {
                            var codeForImage = !string.IsNullOrWhiteSpace(finalItemCode) ? finalItemCode
                                : !string.IsNullOrWhiteSpace(p.ItemCode)   ? p.ItemCode!
                                : "ITEM";
                            var newFileName = $"{codeForImage}{Path.GetExtension(tempFileName)}";
                            var newFullPath = Path.Combine(Path.GetDirectoryName(tempFullPath)!, newFileName);

                            File.Move(tempFullPath, newFullPath, overwrite: true);
                            await _itemRepo.UpdateItemImageAsync(itemId, newFileName, ct);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to move/rename item image for ItemId={ItemId}", itemId);
                    }
                }

                return itemId;
                }
 catch (NotImplementedException nie)
    {
        _logger.LogError(nie, "NotImplemented in CreateItemCommandHandler");
        throw; // let your exception middleware return 500 in dev with full stack
    }
        }

        // helpers ----------
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
            => $"{baseName} {string.Join(" / ", combo.Values)}".Trim();

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

        private static void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
