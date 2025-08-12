// Infrastructure/Repositories/Item/ItemDetail/Commands/ItemUomCommandRepository.cs
using Core.Application.Common.Interfaces.Item.ItemDetail.Commands;
using Core.Application.Item.ItemDetail.Queries.GetAllItems;
using Core.Domain.Entities.Item.ItemDetail;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Infrastructure.Repositories.Item.ItemDetail.Commands
{
    public sealed class ItemUomCommandRepository : IItemUomCommandRepository
    {
        private readonly ApplicationDbContext _db;
        public ItemUomCommandRepository(ApplicationDbContext db) => _db = db;

        public async Task<List<ItemUomDto>> GetByItemIdAsync(int itemId, CancellationToken ct = default)
        {
            return await _db.ItemUOMs.AsNoTracking()
                .Where(x => x.ItemId == itemId)
                .Select(x => new ItemUomDto
                {
                    BaseUOMId = x.BaseUOMId,
                    ConversionUOMId = x.ConversionUOMId,
                    ConversionRate = x.ConversionRate
                })
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(int itemId, IEnumerable<ItemUomDto> rows, CancellationToken ct = default)
        {
            var existing = await _db.ItemUOMs
                .Where(x => x.ItemId == itemId)
                .ToListAsync(ct);

            var map = existing.ToDictionary(k => k.ConversionUOMId!.Value);
            var seen = new HashSet<int>();

            foreach (var dto in rows)
            {
                if (!dto.ConversionUOMId.HasValue)
                    throw new InvalidOperationException("ConversionUOMId is required for UOM row.");

                var key = dto.ConversionUOMId.Value;
                seen.Add(key);

                if (map.TryGetValue(key, out var row))
                {
                    row.BaseUOMId = dto.BaseUOMId;
                    row.ConversionRate = dto.ConversionRate;
                    // EF will track modified values
                }
                else
                {
                    await _db.ItemUOMs.AddAsync(new ItemUOM
                    {
                        ItemId = itemId,
                        BaseUOMId = dto.BaseUOMId,
                        ConversionUOMId = dto.ConversionUOMId,
                        ConversionRate = dto.ConversionRate
                    }, ct);
                }
            }

            // Remove rows not present anymore
            foreach (var row in existing)
            {
                if (row.ConversionUOMId.HasValue && !seen.Contains(row.ConversionUOMId.Value))
                {
                    _db.ItemUOMs.Remove(row);
                }
            }
        }
    }
}
