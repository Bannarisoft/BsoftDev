// Core.Application/Common/Interfaces/Item/ItemDetail/General/IItemGeneralCommandRepository.cs

using Core.Domain.Entities.Item.ItemDetail;

namespace Core.Application.Common.Interfaces.Item.ItemDetail.Commands
{
    public interface IItemGeneralCommandRepository
    {
        Task CreateAsync(ItemGeneral general, CancellationToken ct = default);
        Task UpdateAsync(ItemGeneral entity, CancellationToken ct = default);        
    }
}
