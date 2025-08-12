using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemQualityCommandRepository
    {
        Task CreateAsync(ItemQuality quality, CancellationToken ct = default);
        Task UpdateAsync(ItemQuality entity, CancellationToken ct = default);        
    }
}
