using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Entities.Item.ItemDetail.Variant;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class ItemVariantValueCommandRepository : IItemVariantValueCommandRepository
{
    private readonly ApplicationDbContext _db;
    public ItemVariantValueCommandRepository(ApplicationDbContext db) => _db = db;

    public Task<List<ItemVariantValue>> GetForItemAsync(int itemId, CancellationToken ct = default) =>
        _db.Set<ItemVariantValue>().Where(x => x.ItemId == itemId).AsNoTracking().ToListAsync(ct);

    public async Task UpsertListAsync(int itemId, IEnumerable<VariantValueDto> values, CancellationToken ct = default)
    {
        var incoming = (values ?? Enumerable.Empty<VariantValueDto>())
            .Where(v => v is not null && !string.IsNullOrWhiteSpace(v.OptionValue))
            .Select(v => new VariantValueDto
            {
                AttributeId     = v.AttributeId,
                VariantBasedOn  = v.VariantBasedOn,
                AttributeGroupId= v.AttributeGroupId,     // keep it
                OptionValue     = v.OptionValue.Trim()
            })
            .ToList();

        // --- Validate FKs: AttributeId, VariantBasedOn, AttributeGroupId (if present) ---
        var miscIds = incoming
            .Select(x => x.AttributeId)
            .Concat(incoming.Select(x => x.VariantBasedOn))
            //.Concat(incoming.Where(x => x.AttributeGroupId.HasValue).Select(x => x.AttributeGroupId!.Value))
            .Where(id => id > 0)
            .ToHashSet();

        if (miscIds.Count > 0)
        {
            var found = await _db.MiscMaster.Where(m => miscIds.Contains(m.Id))
                                            .Select(m => m.Id).ToListAsync(ct);
            var missing = miscIds.Except(found).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"Invalid MiscMaster Id(s) in variant payload: {string.Join(", ", missing)}.");
        }

        // --- Upsert by (ItemId, AttributeId, OptionValue normalized) ---
        var existing = await _db.Set<ItemVariantValue>()
                                .Where(x => x.ItemId == itemId)
                                .ToListAsync(ct);

        static string Key(int item, int attr, string val)
            => $"{item}|{attr}|{(val ?? string.Empty).Trim().ToLowerInvariant()}";

        var existingByKey = existing.ToDictionary(
            e => Key(e.ItemId, e.AttributeId, e.OptionValue),
            e => e,
            StringComparer.Ordinal);

        var incomingKeys = incoming.Select(v => Key(itemId, v.AttributeId, v.OptionValue))
                                   .ToHashSet(StringComparer.Ordinal);

        // delete removed options
        var toDelete = existing.Where(e => !incomingKeys.Contains(Key(itemId, e.AttributeId, e.OptionValue))).ToList();
        if (toDelete.Count > 0) _db.RemoveRange(toDelete);

        // insert/update
        foreach (var v in incoming)
        {
            var k = Key(itemId, v.AttributeId, v.OptionValue);
            if (existingByKey.TryGetValue(k, out var row))
            {
                if (row.VariantBasedOn != v.VariantBasedOn)
                    row.VariantBasedOn = v.VariantBasedOn;

                if (row.AttributeGroupId != v.AttributeGroupId)
                    row.AttributeGroupId = v.AttributeGroupId;
            }
            else
            {
                await _db.Set<ItemVariantValue>().AddAsync(new ItemVariantValue
                {
                    ItemId          = itemId,
                    AttributeId     = v.AttributeId,
                    VariantBasedOn  = v.VariantBasedOn,
                    AttributeGroupId= v.AttributeGroupId,     // may be null (OK if DB column is nullable)
                    OptionValue     = v.OptionValue
                }, ct);
            }
        }
        // SaveChanges handled by UoW
    }

    public async Task AddAsync(int itemId, int attributeId, int variantBasedOn, int attributeGroupId, string optionValue, CancellationToken ct = default)
    {
        await ValidateMiscIdsAsync(new[] { attributeId, variantBasedOn, attributeGroupId }, ct);

        var val = (optionValue ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(val))
            throw new ArgumentException("Option value cannot be empty.", nameof(optionValue));

        var exists = await _db.Set<ItemVariantValue>().AnyAsync(
            x => x.ItemId == itemId && x.AttributeId == attributeId && x.OptionValue == val, ct);

        if (!exists)
        {
            await _db.Set<ItemVariantValue>().AddAsync(new ItemVariantValue
            {
                ItemId          = itemId,
                AttributeId     = attributeId,
                VariantBasedOn  = variantBasedOn,
                AttributeGroupId= attributeGroupId,
                OptionValue     = val
            }, ct);
        }
    }

    public async Task SetSingleAsync(int itemId, int attributeId, int variantBasedOn, int attributeGroupId, string optionValue, CancellationToken ct = default)
    {
        await ValidateMiscIdsAsync(new[] { attributeId, variantBasedOn, attributeGroupId }, ct);

        var val = (optionValue ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(val))
            throw new ArgumentException("Option value cannot be empty.", nameof(optionValue));

        var old = await _db.Set<ItemVariantValue>()
                           .Where(x => x.ItemId == itemId && x.AttributeId == attributeId)
                           .ToListAsync(ct);
        if (old.Count > 0) _db.RemoveRange(old);

        await _db.Set<ItemVariantValue>().AddAsync(new ItemVariantValue
        {
            ItemId          = itemId,
            AttributeId     = attributeId,
            VariantBasedOn  = variantBasedOn,
            AttributeGroupId= attributeGroupId,
            OptionValue     = val
        }, ct);
    }

    public async Task RemoveAsync(int itemId, int attributeId, int variantBasedOn, int attributeGroupId, string optionValue, CancellationToken ct = default)
    {
        var val = (optionValue ?? string.Empty).Trim();
        var row = await _db.Set<ItemVariantValue>()
                           .FirstOrDefaultAsync(x =>
                                x.ItemId == itemId &&
                                x.AttributeId == attributeId &&
                                x.VariantBasedOn == variantBasedOn &&
                                x.AttributeGroupId == attributeGroupId &&
                                x.OptionValue == val, ct);

        if (row is not null) _db.Remove(row);
    }

    public async Task ClearForItemAsync(int itemId, CancellationToken ct = default)
    {
        var rows = await _db.Set<ItemVariantValue>()
                            .Where(x => x.ItemId == itemId)
                            .ToListAsync(ct);
        if (rows.Count > 0) _db.RemoveRange(rows);
    }

    // IMPORTANT: only UPDATE NewItemId; do not insert a partial new row here
    public async Task MapOptionToChildAsync(
        int templateItemId,
        int attributeId,
        string optionValue,
        int childItemId,
        CancellationToken ct = default)
    {
        var opt = (optionValue ?? string.Empty).Trim();

        var row = await _db.ItemVariantValue.FirstOrDefaultAsync(
            x => x.ItemId == templateItemId
              && x.AttributeId == attributeId
              && x.OptionValue == opt
              && x.NewItemId == null, ct);

        if (row is null)
            throw new InvalidOperationException(
                $"Template variant not found for ItemId={templateItemId}, Attr={attributeId}, Value='{opt}'. " +
                $"Call AddMissingTemplateOptionsAsync before linking.");

        row.NewItemId = childItemId;
        // No SaveChanges here – let UoW save once
    }

    public async Task AddMissingTemplateOptionsAsync(
        int templateItemId,
        IEnumerable<VariantValueDto> options,
        CancellationToken ct)
    {
        var incoming = options
            .Where(o => o is not null && !string.IsNullOrWhiteSpace(o.OptionValue))
            .Select(o => new
            {
                o.AttributeId,
                Option         = o.OptionValue.Trim(),
                o.VariantBasedOn,
                o.AttributeGroupId
            })
            .ToList();

        if (incoming.Count == 0) return;

        var existing = await _db.ItemVariantValue
            .AsNoTracking()
            .Where(x => x.ItemId == templateItemId && x.NewItemId == null)
            .Select(x => new { x.AttributeId, Option = x.OptionValue })
            .ToListAsync(ct);

        var existSet = new HashSet<(int Attr, string Opt)>(
            existing.Select(e => (e.AttributeId, e.Option.Trim().ToLowerInvariant()))
        );

        foreach (var inc in incoming)
        {
            var key = (inc.AttributeId, inc.Option.ToLowerInvariant());
            if (existSet.Contains(key)) continue;

            await _db.ItemVariantValue.AddAsync(new ItemVariantValue
            {
                ItemId          = templateItemId,
                AttributeId     = inc.AttributeId,
                VariantBasedOn  = inc.VariantBasedOn,
                AttributeGroupId= inc.AttributeGroupId,   // may be null
                OptionValue     = inc.Option,
                NewItemId       = null
            }, ct);

            existSet.Add(key);
        }
        // No SaveChanges here – let UoW save once
    }

    // helpers
    private async Task ValidateMiscIdsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var set = ids.Where(i => i > 0).ToHashSet();
        if (set.Count == 0) return;

        var found = await _db.MiscMaster.Where(m => set.Contains(m.Id))
                                        .Select(m => m.Id).ToListAsync(ct);
        var missing = set.Except(found).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException($"Invalid MiscMaster Id(s): {string.Join(", ", missing)}.");
    }
}
