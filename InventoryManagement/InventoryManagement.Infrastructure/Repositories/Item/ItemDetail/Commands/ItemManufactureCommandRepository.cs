using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public sealed class ItemManufactureCommandRepository : ItemLogRepositoryBase, IItemManufactureCommandRepository
    {
        public ItemManufactureCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }

        public async Task<List<ItemManufactureDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default)
            => await _db.ItemManufacture.AsNoTracking()
                .Where(x => x.ItemId == itemId && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(x => new ItemManufactureDto { UnitId = x.UnitId, ManufacturingTypeId = x.ManufacturingTypeId })
                .ToListAsync(ct);

        public async Task UpdateAsync(int itemId, IEnumerable<ItemManufactureDto> rows, CancellationToken ct = default)
        {
            var existing = await _db.ItemManufacture
                .Where(x => x.ItemId == itemId && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .ToListAsync(ct);

            var map = existing.ToDictionary(k => (k.UnitId, k.ManufacturingTypeId));
            var seen = new HashSet<(int UnitId, int TypeId)>();

            foreach (var dto in rows)
            {
                var key = (dto.UnitId, dto.ManufacturingTypeId);
                seen.Add(key);

                if (map.TryGetValue(key, out var row))
                {
                    var before = new ItemManufacture { ItemId = row.ItemId, UnitId = row.UnitId, ManufacturingTypeId = row.ManufacturingTypeId, IsActive = row.IsActive };
                    row.IsActive = BaseEntity.Status.Active;
                    row.ModifiedDate = DateTimeOffset.UtcNow; row.ModifiedBy = _ctx.CreatedBy ?? row.ModifiedBy; row.ModifiedByName = _ctx.CreatedByName ?? row.ModifiedByName; row.ModifiedIP = _ctx.CreatedIP ?? row.ModifiedIP;
                    AddUpdateLog(nameof(ItemManufacture), row.ItemId, DiffByReflection(before, row));
                }
                else
                {
                    var add = new ItemManufacture
                    {
                        ItemId = itemId,
                        UnitId = dto.UnitId,
                        ManufacturingTypeId = dto.ManufacturingTypeId,
                        IsActive = BaseEntity.Status.Active,
                        IsDeleted = BaseEntity.IsDelete.NotDeleted,
                        CreatedDate = DateTimeOffset.UtcNow,
                        CreatedBy = _ctx.CreatedBy ?? 0,
                        CreatedByName = _ctx.CreatedByName,
                        CreatedIP = _ctx.CreatedIP
                    };
                    await _db.ItemManufacture.AddAsync(add, ct);
                    AddUpdateLog(nameof(ItemManufacture), itemId, DiffByReflection(new ItemManufacture { ItemId = itemId, UnitId = dto.UnitId, ManufacturingTypeId = dto.ManufacturingTypeId }, add));
                }
            }

            foreach (var row in existing)
            {
                if (seen.Contains((row.UnitId, row.ManufacturingTypeId))) continue;
                var before = new ItemManufacture { ItemId = row.ItemId, UnitId = row.UnitId, ManufacturingTypeId = row.ManufacturingTypeId, IsActive = row.IsActive };
                row.IsActive = BaseEntity.Status.Inactive;
                row.ModifiedDate = DateTimeOffset.UtcNow; row.ModifiedBy = _ctx.CreatedBy ?? row.ModifiedBy; row.ModifiedByName = _ctx.CreatedByName ?? row.ModifiedByName; row.ModifiedIP = _ctx.CreatedIP ?? row.ModifiedIP;
                AddUpdateLog(nameof(ItemManufacture), row.ItemId, DiffByReflection(before, row));
            }
        }
    }
}