using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class ItemInventoryCommandRepository : ItemLogRepositoryBase,IItemInventoryCommandRepository
    {        
        public ItemInventoryCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }
        public async Task CreateAsync(ItemInventory inventory, CancellationToken ct = default)
        {
            await _db.ItemInventory.AddAsync(inventory, ct);
        }

        public async Task UpdateAsync(ItemInventory entity, CancellationToken ct = default)
        {               
            var existing = await _db.ItemInventory
                .AsTracking()
                .FirstOrDefaultAsync(x => x.ItemId == entity.ItemId, ct);

            if (existing is null)
            {                
                await _db.ItemInventory.AddAsync(entity, ct);
                return;
            }

            _db.Entry(existing).CurrentValues.SetValues(entity);

            _db.Entry(existing).Property(x => x.ItemId).IsModified = false;
        }
    }
}
