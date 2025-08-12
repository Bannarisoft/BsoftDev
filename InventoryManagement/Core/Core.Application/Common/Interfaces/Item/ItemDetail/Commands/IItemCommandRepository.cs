using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemCommandRepository
    {
        Task<int> CreateAsync(ItemMaster item, CancellationToken ct = default);
        Task UpdateAsync(ItemMaster item, CancellationToken ct = default);
        Task<ItemMaster?> GetTrackingAsync(int id, CancellationToken ct = default); // for update map
        Task<bool> ExistsByCodeForUpdateAsync(string itemCode, int excludeId, CancellationToken ct = default);
        Task<bool> ExistsByCodeForCreateAsync(string itemCode, CancellationToken ct = default);        
    }
}
