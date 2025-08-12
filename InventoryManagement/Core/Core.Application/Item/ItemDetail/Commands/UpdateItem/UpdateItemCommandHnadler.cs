using AutoMapper;
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Events;
using MediatR;

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
        private readonly IItemVariantDefCommandRepository _variantRepo;
        private readonly IItemUomCommandRepository _uomRepo;

        public UpdateItemCommandHandler(
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
            _supplierRepo = supplierRepo; _manuRepo = manuRepo; _variantRepo = variantRepo;_uomRepo = uomRepo;
        }

        public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var p = request.Payload;

            await _uow.BeginTransactionAsync(ct);
            try
            {
                var entity = await _itemRepo.GetTrackingAsync(request.Id, ct)
                            ?? throw new KeyNotFoundException("Item not found.");

                if (await _itemRepo.ExistsByCodeForUpdateAsync(p.ItemCode, request.Id, ct))
                    throw new InvalidOperationException("Another item with same ItemCode exists.");

                _mapper.Map(p, entity);
                await _itemRepo.UpdateAsync(entity, ct);

                if (p.Purchase is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemPurchase>(p.Purchase);
                    e.ItemId = request.Id; await _purchaseRepo.UpdateAsync(e, ct);
                }
                if (p.Inventory is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemInventory>(p.Inventory);
                    e.ItemId = request.Id; await _inventoryRepo.UpdateAsync(e, ct);
                }
                if (p.Quality is not null)
                {
                    var e = _mapper.Map<Core.Domain.Entities.Item.ItemDetail.ItemQuality>(p.Quality);
                    e.ItemId = request.Id; await _qualityRepo.UpdateAsync(e, ct);
                }

                // Collections (upsert list; deactivate missing)
                await _supplierRepo.UpdateAsync(request.Id, p.Suppliers, ct);
                await _manuRepo.UpdateAsync(request.Id, p.Manufacture, ct);
                //UOM
                if (p.Uoms.Count > 0)
                {
                    await _uomRepo.UpdateAsync(request.Id, p.Uoms, ct);
                }

                if (p.HasVariants && p.VariantDefs is not null && p.VariantDefs.Count > 0)
                {
                    var defs = p.VariantDefs.Select(d => (d.AttributeId, d.OptionIds.AsEnumerable()));
                    await _variantRepo.ReplaceDefsAsync(request.Id, defs, ct);
                }

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync(ct);

                await _mediator.Publish(new AuditLogsDomainEvent(
                    "Update", request.Id.ToString(), p.ItemName, "Item updated with tabs & child lists", "ItemMaster"), ct);

                return Unit.Value;
            }
            catch
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }
    }
}
