// InventoryManagement.Infrastructure/Repositories/Item/ItemDetail/Variant/ItemVariantValueQueryRepository.cs
using Core.Application.Common.Interfaces.Item.ItemDetail.Queries;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;   // VariantValueDto
using Core.Domain.Common;
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Variant
{
    public sealed class ItemVariantValueQueryRepository : IItemVariantValueQueryRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemVariantValueQueryRepository(ApplicationDbContext db) => _db = db;

        /// <summary>
        /// Returns attributeId -> distinct option values (case-sensitive distinct; ordered).
        /// </summary>
        public async Task<Dictionary<int, List<string>>> GetForItemGroupedAsync(
            int itemId, CancellationToken ct = default)
        {
            // Simple, fully translatable query.
            var rows = await _db.Set<ItemVariantValue>()
                .Where(v => v.ItemId == itemId)
                .Select(v => new { v.AttributeId, v.OptionValue })
                .ToListAsync(ct);

            return rows
                .GroupBy(x => x.AttributeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => (x.OptionValue ?? string.Empty).Trim())
                          .Where(s => s != string.Empty)
                          .Distinct()               // EF distinct already materialized above
                          .OrderBy(s => s)
                          .ToList()
                );
        }

        /// <summary>
        /// For a template item, builds a set of combo keys for all current children.
        /// Key format: "attrId:value|attrId:value" ordered by attrId, value normalized (trim+lower).
        /// </summary>
        public async Task<HashSet<string>> GetExistingChildComboKeysAsync(
            int templateItemId, CancellationToken ct = default)
        {
            // Get current (not-deleted) child ids
            var childIds = await _db.Set<ItemMaster>()
                .Where(i => i.ParentItemId == templateItemId &&
                            i.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(i => i.Id)
                .ToListAsync(ct);

            if (childIds.Count == 0) return new HashSet<string>();

            var rows = await _db.Set<ItemVariantValue>()
                .Where(v => childIds.Contains(v.ItemId))
                .Select(v => new { v.ItemId, v.AttributeId, v.OptionValue })
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

        /// <summary>
        /// Same as above, but returns a map key -> childId.
        /// </summary>
        public async Task<Dictionary<string, int>> GetExistingChildCombosWithIdsAsync(
            int templateItemId, CancellationToken ct = default)
        {
            var childIds = await _db.Set<ItemMaster>()
                .Where(i => i.ParentItemId == templateItemId &&
                            i.IsDeleted == BaseEntity.IsDelete.NotDeleted)
                .Select(i => i.Id)
                .ToListAsync(ct);

            if (childIds.Count == 0) return new Dictionary<string, int>();

            var rows = await _db.Set<ItemVariantValue>()
                .Where(v => childIds.Contains(v.ItemId))
                .Select(v => new { v.ItemId, v.AttributeId, v.OptionValue })
                .ToListAsync(ct);

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

        /// <summary>
        /// Returns the raw variant rows (AttributeId, OptionValue, VariantBasedOn) for an item.
        /// </summary>
        public Task<List<VariantValueDto>> GetForItemAsync(
            int itemId, CancellationToken ct = default)
        {
            return _db.Set<ItemVariantValue>()
                .Where(v => v.ItemId == itemId)
                .OrderBy(v => v.AttributeId).ThenBy(v => v.OptionValue)
                .Select(v => new VariantValueDto
                {
                    AttributeId = v.AttributeId,
                    OptionValue = v.OptionValue,
                    VariantBasedOn = v.VariantBasedOn,
                    AttributeGroupId = v.AttributeGroupId
                })
                .ToListAsync(ct);
        }

        private static string Normalize(string? s)
            => (s ?? string.Empty).Trim().ToLowerInvariant();
    }
}
