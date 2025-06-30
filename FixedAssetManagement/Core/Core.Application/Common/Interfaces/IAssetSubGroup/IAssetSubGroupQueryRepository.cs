namespace Core.Application.Common.Interfaces.IAssetSubGroup
{
    public interface IAssetSubGroupQueryRepository
    {
        Task<Core.Domain.Entities.AssetSubGroup?> GetByIdAsync(int Id);
        Task<List<Core.Domain.Entities.AssetSubGroup?>> GetByGroupIdAsync(int GroupId);
        Task<(List<Core.Domain.Entities.AssetSubGroup>, int)> GetAllAssetSubGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<Core.Domain.Entities.AssetSubGroup>> GetAssetSubGroups(string searchPattern);     
    }
}