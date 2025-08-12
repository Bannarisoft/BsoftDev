using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class ItemPurchaseCommandRepository : ItemLogRepositoryBase,IItemPurchaseCommandRepository
    {   
        public ItemPurchaseCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }
        public async Task CreateAsync(ItemPurchase purchase, CancellationToken ct = default)
        {
            await _db.ItemPurchase.AddAsync(purchase, ct);
        }

        public async Task UpdateAsync(ItemPurchase updated, CancellationToken ct = default)
        {
            var existing = await _db.ItemPurchase.FirstOrDefaultAsync(x => x.ItemId == updated.ItemId, ct);
            List<PropertyChange> changes; 
            if (existing is null)
            {
                _db.ItemPurchase.Add(updated);
                changes = DiffByReflection(new ItemPurchase { ItemId = updated.ItemId }, updated);
                AddUpdateLog(nameof(ItemPurchase), updated.ItemId, changes);
                return;
            }

            var entry = _db.Entry(existing);
            entry.CurrentValues.SetValues(updated);
            entry.Property(x => x.ItemId).IsModified = false;

            changes = GetModifiedProps(entry);
            AddUpdateLog(nameof(ItemPurchase), existing.ItemId, changes);
        }
    }
}
