using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public sealed class ItemSupplierCommandRepository : ItemLogRepositoryBase, IItemSupplierCommandRepository
    {
        public ItemSupplierCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }

        public async Task<List<ItemSupplierDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default)
            => await _db.ItemSupplier.AsNoTracking()
                .Where(x => x.ItemId == itemId && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(x => new ItemSupplierDto { SupplierId = x.SupplierId, UnitId = x.UnitId, SupplierPartNo = x.SupplierPartNo })
                .ToListAsync(ct);

        public async Task UpdateAsync(int itemId, IEnumerable<ItemSupplierDto> suppliers, CancellationToken ct = default)
        {
            var existing = await _db.ItemSupplier
                .Where(x => x.ItemId == itemId && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .ToListAsync(ct);

            var map = existing.ToDictionary(k => (k.SupplierId, k.UnitId));
            var seen = new HashSet<(int SupplierId, int UnitId)>();

            foreach (var dto in suppliers)
            {
                var key = (dto.SupplierId, dto.UnitId);
                seen.Add(key);

                if (map.TryGetValue(key, out var row))
                {
                    var before = new ItemSupplier { ItemId = row.ItemId, SupplierId = row.SupplierId, UnitId = row.UnitId, SupplierPartNo = row.SupplierPartNo, IsActive = row.IsActive };
                    row.SupplierPartNo = dto.SupplierPartNo;
                    row.IsActive = BaseEntity.Status.Active;
                    row.ModifiedDate = DateTimeOffset.UtcNow; row.ModifiedBy = _ctx.CreatedBy ?? row.ModifiedBy; row.ModifiedByName = _ctx.CreatedByName ?? row.ModifiedByName; row.ModifiedIP = _ctx.CreatedIP ?? row.ModifiedIP;
                    AddUpdateLog(nameof(ItemSupplier), row.ItemId, DiffByReflection(before, row));
                }
                else
                {
                    var add = new ItemSupplier
                    {
                        ItemId = itemId,
                        SupplierId = dto.SupplierId,
                        UnitId = dto.UnitId,
                        SupplierPartNo = dto.SupplierPartNo,
                        IsActive = BaseEntity.Status.Active,
                        IsDeleted = BaseEntity.IsDelete.NotDeleted,
                        CreatedDate = DateTimeOffset.UtcNow,
                        CreatedBy = _ctx.CreatedBy ?? 0,
                        CreatedByName = _ctx.CreatedByName,
                        CreatedIP = _ctx.CreatedIP
                    };
                    await _db.ItemSupplier.AddAsync(add, ct);
                    AddUpdateLog(nameof(ItemSupplier), itemId, DiffByReflection(new ItemSupplier { ItemId = itemId, SupplierId = dto.SupplierId, UnitId = dto.UnitId }, add));
                }
            }

            foreach (var row in existing)
            {
                if (seen.Contains((row.SupplierId, row.UnitId))) continue;
                var before = new ItemSupplier { ItemId = row.ItemId, SupplierId = row.SupplierId, UnitId = row.UnitId, IsActive = row.IsActive };
                row.IsActive = BaseEntity.Status.Inactive;
                row.ModifiedDate = DateTimeOffset.UtcNow; row.ModifiedBy = _ctx.CreatedBy ?? row.ModifiedBy; row.ModifiedByName = _ctx.CreatedByName ?? row.ModifiedByName; row.ModifiedIP = _ctx.CreatedIP ?? row.ModifiedIP;
                AddUpdateLog(nameof(ItemSupplier), row.ItemId, DiffByReflection(before, row));
            }
        }
    }
}