using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;   // VariantValueDto
using Core.Domain.Entities.Item.ItemDetail;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Variant
{
    public class ItemVariantValueCommandRepository : IItemVariantValueCommandRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemVariantValueCommandRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task UpsertListAsync(int itemId, IEnumerable<VariantValueDto> values, CancellationToken ct = default)
        {
            if (values is null) values = Enumerable.Empty<VariantValueDto>();

            // Normalize incoming (trim, ignore blanks)
            var normalized = values
                .Where(v => v is not null && !string.IsNullOrWhiteSpace(v.OptionValue))
                .Select(v => new VariantValueDto
                {
                    AttributeId = v.AttributeId,
                    OptionValue = v.OptionValue.Trim()
                })
                .ToList();

            // Detect child vs template
            var parentId = await _db.Set<ItemMaster>()
                .Where(i => i.Id == itemId)
                .Select(i => i.ParentItemId)
                .FirstOrDefaultAsync(ct);

            var isChild = parentId is not null;

            // For child items: ensure exactly one value per attribute (fail fast if duplicates provided)
            if (isChild)
            {
                var dup = normalized
                    .GroupBy(v => v.AttributeId)
                    .FirstOrDefault(g => g.Count() > 1);

                if (dup is not null)
                    throw new InvalidOperationException($"Child item {itemId} can have exactly one value per attribute. Duplicate for AttributeId={dup.Key}.");
            }

            // Load existing rows for this item
            var existing = await _db.Set<ItemVariantValue>()
                .Where(x => x.ItemId == itemId)
                .ToListAsync(ct);

            // Build lookups (case-insensitive match on OptionValue)
            var incomingKeys = new HashSet<string>(
                normalized.Select(v => Key(itemId, v.AttributeId, v.OptionValue)),
                StringComparer.OrdinalIgnoreCase);

            var existingKeys = new HashSet<string>(
                existing.Select(x => Key(itemId, x.AttributeId, x.OptionValue)),
                StringComparer.OrdinalIgnoreCase);

            // Rows to insert (in incoming but not in existing)
            var toInsert = normalized
                .Where(v => !existingKeys.Contains(Key(itemId, v.AttributeId, v.OptionValue)))
                .Select(v => new ItemVariantValue
                {
                    ItemId = itemId,
                    AttributeId = v.AttributeId,
                    OptionValue = v.OptionValue.Trim()
                })
                .ToList();

            // Rows to delete (in existing but not in incoming) — full replace/sync behavior
            var toDelete = existing
                .Where(x => !incomingKeys.Contains(Key(itemId, x.AttributeId, x.OptionValue)))
                .ToList();

            if (toDelete.Count > 0)
                _db.RemoveRange(toDelete);

            if (toInsert.Count > 0)
                await _db.AddRangeAsync(toInsert, ct);

            // No SaveChanges here; UoW handles it.
        }

        public async Task AddAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default)
        {
            var val = Normalize(optionValue);
            if (string.IsNullOrEmpty(val))
                throw new ArgumentException("Option value cannot be empty.", nameof(optionValue));

            var exists = await _db.Set<ItemVariantValue>().AnyAsync(
                x => x.ItemId == itemId && x.AttributeId == attributeId && x.OptionValue == val, ct);

            if (!exists)
            {
                await _db.Set<ItemVariantValue>().AddAsync(new ItemVariantValue
                {
                    ItemId = itemId,
                    AttributeId = attributeId,
                    OptionValue = val
                }, ct);
            }
        }

        public async Task SetSingleAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default)
        {
            var val = Normalize(optionValue);
            if (string.IsNullOrEmpty(val))
                throw new ArgumentException("Option value cannot be empty.", nameof(optionValue));

            // Delete any existing value(s) for this attribute on this item
            var old = await _db.Set<ItemVariantValue>()
                .Where(x => x.ItemId == itemId && x.AttributeId == attributeId)
                .ToListAsync(ct);

            if (old.Count > 0)
                _db.RemoveRange(old);

            // Insert the single value
            await _db.Set<ItemVariantValue>().AddAsync(new ItemVariantValue
            {
                ItemId = itemId,
                AttributeId = attributeId,
                OptionValue = val
            }, ct);
        }

        public async Task RemoveAsync(int itemId, int attributeId, string optionValue, CancellationToken ct = default)
        {
            var val = Normalize(optionValue);
            var row = await _db.Set<ItemVariantValue>()
                .FirstOrDefaultAsync(x => x.ItemId == itemId && x.AttributeId == attributeId && x.OptionValue == val, ct);

            if (row is not null)
                _db.Remove(row);
        }

        public async Task ClearForItemAsync(int itemId, CancellationToken ct = default)
        {
            var rows = await _db.Set<ItemVariantValue>()
                .Where(x => x.ItemId == itemId)
                .ToListAsync(ct);

            if (rows.Count > 0)
                _db.RemoveRange(rows);
        }

        private static string Normalize(string s) => s?.Trim() ?? string.Empty;

        private static string Key(int itemId, int attributeId, string value)
            => $"{itemId}|{attributeId}|{value?.Trim()}";
    }
}
