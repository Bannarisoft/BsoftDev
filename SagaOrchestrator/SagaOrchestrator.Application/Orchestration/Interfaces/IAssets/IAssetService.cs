using Contracts.Dtos.Users;

namespace SagaOrchestrator.Application.Orchestration.Interfaces.IAssets
{
    public interface IAssetService
    {
        Task<AssetDto> GetAssetByIdAsync(int assetId);
    }
}