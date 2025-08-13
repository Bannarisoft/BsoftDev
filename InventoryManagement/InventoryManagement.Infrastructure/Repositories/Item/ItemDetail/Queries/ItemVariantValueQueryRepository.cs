// InventoryManagement.Infrastructure/Repositories/Item/ItemDetail/Variant/ItemVariantValueQueryRepository.cs
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Variant
{
    public sealed class ItemVariantValueQueryRepository : IItemVariantValueQueryRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemVariantValueQueryRepository(ApplicationDbContext db) => _db = db;

        public async Task<Dictionary<int, List<string>>> GetForItemGroupedAsync(int itemId, CancellationToken ct = default)
        {
            return await _db.Set<ItemVariantValue>()
                .Where(v => v.ItemId == itemId)
                .GroupBy(v => v.AttributeId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Select(x => x.OptionValue).Distinct().OrderBy(x => x).ToList(),
                    ct);
        }

        public async Task<HashSet<string>> GetExistingChildComboKeysAsync(int templateItemId, CancellationToken ct = default)
        {
            var childIds = await _db.Set<ItemMaster>()
                .Where(i => i.ParentItemId == templateItemId && i.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted)
                .Select(i => i.Id)
                .ToListAsync(ct);

            if (childIds.Count == 0) return new HashSet<string>();

            var rows = await _db.Set<ItemVariantValue>()
                .Where(v => childIds.Contains(v.ItemId))
                .ToListAsync(ct);

            var keys = rows
                .GroupBy(r => r.ItemId)
                .Select(g =>
                {
                    var parts = g.OrderBy(x => x.AttributeId)
                                 .Select(x => $"{x.AttributeId}:{Normalize(x.OptionValue)}");
                    return string.Join('|', parts);
                });

            return new HashSet<string>(keys);
        }
     
        public async Task<Dictionary<string, int>> GetExistingChildCombosWithIdsAsync(int templateItemId, CancellationToken ct = default)
        {
            var childIds = await _db.Set<ItemMaster>()
                .Where(i => i.ParentItemId == templateItemId && i.IsDeleted == Core.Domain.Common.BaseEntity.IsDelete.NotDeleted)
                .Select(i => i.Id)
                .ToListAsync(ct);

            if (childIds.Count == 0) return new Dictionary<string, int>();

            var rows = await _db.Set<ItemVariantValue>()
                .Where(v => childIds.Contains(v.ItemId))
                .ToListAsync(ct);

            // one key per child
            return rows
                .GroupBy(r => r.ItemId)
                .ToDictionary(
                    g =>
                    {
                        var parts = g.OrderBy(x => x.AttributeId)
                                     .Select(x => $"{x.AttributeId}:{Normalize(x.OptionValue)}");
                        return string.Join('|', parts);
                    },
                    g => g.Key // childId
                );
        }
        private static string Normalize(string s) => (s ?? string.Empty).Trim().ToLowerInvariant();
    }
}
