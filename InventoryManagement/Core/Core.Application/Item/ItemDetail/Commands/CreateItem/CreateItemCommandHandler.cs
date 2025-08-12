using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Commands.CreateItem;
using Core.Domain.Events;
using MediatR;

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
        private readonly IItemVariantDefCommandRepository _variantRepo;
        private readonly IItemUomCommandRepository _uomRepo;

        public CreateItemCommandHandler(
            IUnitOfWork uow, IMapper mapper, IMediator mediator,
            IItemCommandRepository itemRepo,
            IItemPurchaseCommandRepository purchaseRepo,
            IItemInventoryCommandRepository inventoryRepo,
            IItemQualityCommandRepository qualityRepo,
            IItemSupplierCommandRepository supplierRepo,
            IItemManufactureCommandRepository manuRepo,
            IItemVariantDefCommandRepository variantRepo, IItemUomCommandRepository uomRepo)
        {
            _uow = uow; _mapper = mapper; _mediator = mediator;
            _itemRepo = itemRepo; _purchaseRepo = purchaseRepo; _inventoryRepo = inventoryRepo; _qualityRepo = qualityRepo;
            _supplierRepo = supplierRepo; _manuRepo = manuRepo; _variantRepo = variantRepo; _uomRepo = uomRepo;
        }

        public async Task<int> Handle(CreateItemCommand request, CancellationToken ct)
        {
            var p = request.Payload;

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // 1) Base
                var item = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemMaster>(p);
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

                // 4) Variants
                if (p.HasVariants && p.VariantDefs is not null && p.VariantDefs.Count > 0)
                {
                    var defs = p.VariantDefs.Select(d => (d.AttributeId, d.OptionIds.AsEnumerable()));
                    await _variantRepo.ReplaceDefsAsync(itemId, defs, ct); // adjust to your behavior
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                    "Create", itemId.ToString(), p.ItemName, "Item created with tabs & child lists", "ItemMaster"), ct);

                return itemId;
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }
    }
}
