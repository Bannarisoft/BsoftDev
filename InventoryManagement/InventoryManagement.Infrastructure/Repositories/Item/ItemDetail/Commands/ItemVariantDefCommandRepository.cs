using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class ItemVariantDefCommandRepository : IItemVariantDefCommandRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemVariantDefCommandRepository(ApplicationDbContext db) => _db = db;

        public async Task<int> CreateDefAsync(int itemId, int attributeId, CancellationToken ct = default)
        {
            var def = new ItemVariantDef
            {
                ItemId = itemId,
                AttributeId = attributeId
            };
            await _db.ItemVariantDef.AddAsync(def, ct);
            await _db.SaveChangesAsync(ct); // need Id for options
            return def.Id;
        }

        public async Task AddDefOptionsAsync(int itemVariantDefId, IEnumerable<int> optionIds, CancellationToken ct = default)
        {
            foreach (var optId in optionIds.Distinct())
            {
                await _db.ItemVariantDefOption.AddAsync(new ItemVariantDefOption
                {
                    ItemVariantDefId = itemVariantDefId,
                    OptionId = optId
                }, ct);
            }
            // Save is coordinated by UnitOfWork in the handler; no SaveChanges here.
        }
        public async Task ReplaceDefsAsync(int itemId,
            IEnumerable<(int attributeId, IEnumerable<int> optionIds)> defs,
            CancellationToken ct = default)
        {
            // Remove old defs/options
            var oldDefs = await _db.ItemVariantDef.Where(d => d.ItemId == itemId).ToListAsync(ct);
            if (oldDefs.Count > 0)
            {
                var oldDefIds = oldDefs.Select(d => d.Id).ToList();
                var oldOpts = _db.ItemVariantDefOption.Where(o => oldDefIds.Contains(o.ItemVariantDefId));
                _db.ItemVariantDefOption.RemoveRange(oldOpts);
                _db.ItemVariantDef.RemoveRange(oldDefs);
            }

            // Add new defs + options
            foreach (var (attributeId, optionIds) in defs)
            {
                var def = new ItemVariantDef { ItemId = itemId, AttributeId = attributeId };
                await _db.ItemVariantDef.AddAsync(def, ct);
                await _db.SaveChangesAsync(ct); // need def.Id

                if (optionIds != null)
                {
                    foreach (var opt in optionIds.Distinct())
                        await _db.ItemVariantDefOption.AddAsync(new ItemVariantDefOption
                        { ItemVariantDefId = def.Id, OptionId = opt }, ct);
                }
            }
            // final Save done by UoW
        }

        public async Task ClearDefsAsync(int itemId, CancellationToken ct = default)
        {
            var defs = await _db.ItemVariantDef.Where(d => d.ItemId == itemId).ToListAsync(ct);
            if (defs.Count == 0) return;
            var defIds = defs.Select(d => d.Id).ToList();
            var opts = _db.ItemVariantDefOption.Where(o => defIds.Contains(o.ItemVariantDefId));
            _db.ItemVariantDefOption.RemoveRange(opts);
            _db.ItemVariantDef.RemoveRange(defs);
        }
    }
}
