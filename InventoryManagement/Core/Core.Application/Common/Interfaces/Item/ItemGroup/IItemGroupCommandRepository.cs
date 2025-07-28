namespace Core.Application.Common.Interfaces.Item.ItemGroup
{
    public interface IItemGroupCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.Item.ItemGroup itemGroup);
        Task<int>  UpdateAsync(int assetId,Domain.Entities.Item.ItemGroup itemGroup);
        Task<int>  DeleteAsync(int assetId,Domain.Entities.Item.ItemGroup itemGroup);       
        Task<bool> IsNameDuplicateAsync(string? name,int excludeId);
    }
}