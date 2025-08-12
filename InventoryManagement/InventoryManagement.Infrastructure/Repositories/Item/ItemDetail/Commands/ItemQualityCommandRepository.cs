using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class ItemQualityCommandRepository :ItemLogRepositoryBase, IItemQualityCommandRepository
    {             
        public ItemQualityCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }

        public async Task CreateAsync(ItemQuality quality, CancellationToken ct = default)
        {
            await _db.ItemQuality.AddAsync(quality, ct);
        }

        public async Task UpdateAsync(ItemQuality updated, CancellationToken ct = default)
        {
            var existing = await _db.ItemQuality.FirstOrDefaultAsync(x => x.ItemId == updated.ItemId, ct);
            List<PropertyChange> changes; 
            if (existing is null)
            {
                _db.ItemQuality.Add(updated);
                changes = DiffByReflection(new ItemQuality { ItemId = updated.ItemId }, updated);
                AddUpdateLog(nameof(ItemQuality), updated.ItemId, changes);
                return;
            }

            var entry = _db.Entry(existing);
            entry.CurrentValues.SetValues(updated);
            entry.Property(x => x.ItemId).IsModified = false;

            changes = GetModifiedProps(entry);
            AddUpdateLog(nameof(ItemQuality), existing.ItemId, changes);
        }
    }
}
