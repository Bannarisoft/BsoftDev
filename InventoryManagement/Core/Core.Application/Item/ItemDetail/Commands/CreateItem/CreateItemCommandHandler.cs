using System.Text;
using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
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
        private readonly IItemCommandRepository _itemRepo;
        private readonly IItemPurchaseCommandRepository _purchaseRepo;
        private readonly IItemInventoryCommandRepository _inventoryRepo;
        private readonly IItemQualityCommandRepository _qualityRepo;
        private readonly IItemSupplierCommandRepository _supplierRepo;
        private readonly IItemManufactureCommandRepository _manuRepo;
        private readonly IItemUomCommandRepository _uomRepo;
        private readonly IItemQueryRepository _itemQueryRepository;
        private readonly ILogger<CreateItemCommandHandler> _logger;

        private readonly IItemVariantValueCommandRepository _variantCmd;
        private readonly IItemVariantValueQueryRepository _variantQry;

        public CreateItemCommandHandler(
            IUnitOfWork uow, IMapper mapper, IMediator mediator,
            IItemCommandRepository itemRepo,
            IItemPurchaseCommandRepository purchaseRepo,
            IItemInventoryCommandRepository inventoryRepo,
            IItemQualityCommandRepository qualityRepo,
            IItemSupplierCommandRepository supplierRepo,
            IItemManufactureCommandRepository manuRepo,
            IItemUomCommandRepository uomRepo, IItemQueryRepository itemQueryRepository,ILogger<CreateItemCommandHandler> logger,  IItemVariantValueCommandRepository variantCmd, IItemVariantValueQueryRepository variantQry)
        {
            _uow = uow; _mapper = mapper; _mediator = mediator;
            _itemRepo = itemRepo; _purchaseRepo = purchaseRepo; _inventoryRepo = inventoryRepo; _qualityRepo = qualityRepo;
            _supplierRepo = supplierRepo; _manuRepo = manuRepo; _uomRepo = uomRepo; _itemQueryRepository = itemQueryRepository;  _logger = logger;_variantCmd = variantCmd; _variantQry = variantQry;
        }

        public async Task<int> Handle(CreateItemCommand request, CancellationToken ct)
        {
            var p = request.Payload;

            await _uow.BeginTransactionAsync(ct);
            try
            {
                if (!p.ItemGroupId.HasValue || !p.ItemCategoryId.HasValue)
                    throw new InvalidOperationException("ItemGroupId and ItemCategoryId are required to generate ItemCode.");

                // 1) Base
                
                var itemCode = await _itemQueryRepository.GetLatestItemCode(p.ItemGroupId.Value, p.ItemCategoryId.Value, ct);

                if (await _itemRepo.ExistsByCodeForCreateAsync(itemCode, ct))
                    throw new InvalidOperationException($"Generated ItemCode '{itemCode}' already exists.");

                
                var item = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(p);
                item.ItemCode = itemCode;
                var itemId = await _itemRepo.CreateAsync(item, ct); // saves once to get Id

                // 2) Tabs
                if (p.Purchase is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                    e.ItemId = itemId; await _purchaseRepo.CreateAsync(e, ct);
                }
                if (p.Inventory is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                    e.ItemId = itemId; await _inventoryRepo.CreateAsync(e, ct);
                }
                if (p.Quality is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                    e.ItemId = itemId; await _qualityRepo.CreateAsync(e, ct);
                }

                // 3) Collections
                if (p.Suppliers.Count > 0) await _supplierRepo.UpdateAsync(itemId, p.Suppliers, ct);
                if (p.Manufacture.Count > 0) await _manuRepo.UpdateAsync(itemId, p.Manufacture, ct);
                //UOM
                if (p.Uoms.Count > 0)
                {
                    await _uomRepo.UpdateAsync(itemId, p.Uoms, ct);
                }

                // 4) Variants on template (same table)
                if (p.VariantValues is { Count: > 0 })
                    await _variantCmd.UpsertListAsync(itemId, p.VariantValues, ct);

                // 5) If HasVariants = true → auto generate children (only ItemName differs)
                if (p.HasVariants)
                {
                    // Read the allowed set from what we just wrote on the template
                    var allowed = await _variantQry.GetForItemGroupedAsync(itemId, ct);
                    if (allowed.Count > 0 && allowed.All(kv => kv.Value.Count > 0))
                    {
                        var combos = Cartesian(allowed); // List<Dictionary<int,string>>

                        // Avoid duplicates if re-run (shouldn’t happen on create, but harmless)
                        var existingComboKeys = await _variantQry.GetExistingChildComboKeysAsync(itemId, ct);

                        foreach (var combo in combos)
                        {
                            var key = ComboKey(combo);
                            if (existingComboKeys.Contains(key)) continue;

                            var childName = BuildChildName(p.ItemName, combo);

                            var child = new Core.Domain.Entities.Item.ItemDetail.ItemMaster
                            {
                                UnitId = item.UnitId,
                                ItemCode = item.ItemCode,   // NOTE: if ItemCode must be unique, add a suffix here.
                                ItemName = childName,       // ONLY change
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
                                HasVariants = false,           // child is concrete
                                ParentItemId = itemId,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted
                            };

                            var childId = await _itemRepo.CreateAsync(child, ct);

                            // Clone tabs (optional – here: yes)
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

                            // Clone collections (optional – here: yes)
                            if (p.Suppliers is { Count: > 0 }) await _supplierRepo.UpdateAsync(childId, p.Suppliers, ct);
                            if (p.Manufacture is { Count: > 0 }) await _manuRepo.UpdateAsync(childId, p.Manufacture, ct);
                            if (p.Uoms is { Count: > 0 }) await _uomRepo.UpdateAsync(childId, p.Uoms, ct);

                            // Write child’s selected values (one per attribute)
                            var selected = combo.Select(kv => new VariantValueDto
                            {
                                AttributeId = kv.Key,
                                OptionValue = kv.Value
                            }).ToList();

                            await _variantCmd.UpsertListAsync(childId, selected, ct);
                        }
                    }
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                    "Create",
                    itemId.ToString(),
                    p.ItemName,
                    p.HasVariants
                        ? "Template + variant children created automatically."
                        : "Single item created (no variants).",
                    "ItemMaster"), ct);

               if (itemId > 0 && !string.IsNullOrWhiteSpace(p.ItemImage))
                {
                    try
                    {
                        var baseDirectory = await _itemQueryRepository.GetBaseDirectoryAsync(ct);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory);

                        var tempFileName = p.ItemImage; // e.g. "tmp/abc123.png"
                        var tempFullPath = Path.Combine(uploadPath, tempFileName);
                        EnsureDirectoryExists(Path.GetDirectoryName(tempFullPath)!);

                        if (File.Exists(tempFullPath))
                        {
                            var newFileName = $"{item.ItemCode}{Path.GetExtension(tempFileName)}";
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
                return itemId; // template Id (children are linked by ParentItemId)
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }

        // ---------- helpers ----------
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
        private void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }     
    }
}
