
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail;
using Dapper;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public class ItemCommandRepository : ItemLogRepositoryBase, IItemCommandRepository
    {
        public ItemCommandRepository(ApplicationDbContext db, IExecutionContext ctx) : base(db, ctx) { }
        public async Task<int> CreateAsync(ItemMaster item, CancellationToken ct = default)
        {
            await _db.ItemMaster.AddAsync(item, ct);
            await _db.SaveChangesAsync(ct);
            return item.Id;
        }
        public async Task UpdateAsync(ItemMaster entity, CancellationToken ct = default)
        {
            var entry = _db.Entry(entity);

            List<PropertyChange> changes;

            if (entry.State == EntityState.Detached)
            {
                // load original (no tracking) to compute diffs
                var original = await _db.ItemMaster.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == entity.Id, ct) ?? throw new KeyNotFoundException("Item not found.");

                changes = DiffByReflection(original, entity);

                _db.ItemMaster.Attach(entity);
                entry = _db.Entry(entity);
                entry.State = EntityState.Modified;
                entry.Property(x => x.Id).IsModified = false; // protect PK
            }
            else
            {
                // already tracked (common path with your handler)
                changes = GetModifiedProps(entry);
            }

            // set audit on master
            entity.ModifiedDate = DateTimeOffset.UtcNow;
            entity.ModifiedBy = _ctx.CreatedBy ?? entity.ModifiedBy;
            entity.ModifiedByName = _ctx.CreatedByName ?? entity.ModifiedByName;
            entity.ModifiedIP = _ctx.CreatedIP ?? entity.ModifiedIP;

            AddUpdateLog(nameof(ItemMaster), entity.Id, changes);
        }
        public async Task<ItemMaster?> GetTrackingAsync(int id, CancellationToken ct = default)
        {
            return await _db.ItemMaster.FirstOrDefaultAsync(x => x.Id == id, ct);
        }
        public async Task<bool> ExistsByCodeForUpdateAsync(string itemCode, int excludeId, CancellationToken ct = default)
        {
            return await _db.ItemMaster
                .AnyAsync(x => x.ItemCode == itemCode && x.Id != excludeId, ct);
        }

        public async Task<bool> ExistsByCodeForCreateAsync(string itemCode, CancellationToken ct = default)
        {
            return await _db.ItemMaster
                .AnyAsync(x => x.ItemCode == itemCode
                               && x.IsDeleted == BaseEntity.IsDelete.NotDeleted, ct);
        }
        public async Task<List<int>> GetChildIdsAsync(int templateItemId, CancellationToken ct = default)
        {
            return await _db.ItemMaster
                .Where(x => x.ParentItemId == templateItemId
                            && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(x => x.Id)
                .ToListAsync(ct);
        }
        public async Task<bool> UpdateItemImageAsync(int itemId, string imageName, CancellationToken ct = default)
        {
            var asset = await _db.ItemMaster.FindAsync(itemId);
            if (asset == null)
            {
                return false;  
            }
            asset.ItemImage = imageName;
            await _db.SaveChangesAsync();
            return true;
        }

         public Task<bool> ExistsByNameSmartForCreateAsync(string name, CancellationToken ct = default)
        {
            var normInput = NormalizeClient(name);
            return _db.ItemMaster
                .Where(x => x.IsDeleted ==BaseEntity.IsDelete.NotDeleted )
                .AnyAsync(x =>
                    NormalizeSql(EF.Functions.Collate(x.ItemName, "Latin1_General_CI_AI")) == normInput, ct);
        }

        public Task<bool> ExistsByNameSmartForUpdateAsync(string name, int excludeId, CancellationToken ct = default)
        {
            var normInput = NormalizeClient(name);
            return _db.ItemMaster
                .Where(x => x.Id != excludeId && x.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .AnyAsync(x =>
                    NormalizeSql(EF.Functions.Collate(x.ItemName, "Latin1_General_CI_AI")) == normInput, ct);
        }

        // Inline-translatable: chain .Replace so EF generates SQL REPLACE(...)
        private static string NormalizeSql(string s) =>
            s.ToLower()
             .Replace(" ", "")
             .Replace("-", "")
             .Replace("_", "")
             .Replace(".", "")
             .Replace("/", "")
             .Replace("\\", "")
             .Replace("'", "")
             .Replace("(", "")
             .Replace(")", "");

        private static string NormalizeClient(string? s)
        {
            s = (s ?? string.Empty).ToLowerInvariant();
            return s.Replace(" ", "")
                    .Replace("-", "")
                    .Replace("_", "")
                    .Replace(".", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace("'", "")
                    .Replace("(", "")
                    .Replace(")", "");
        }    
    }
}
