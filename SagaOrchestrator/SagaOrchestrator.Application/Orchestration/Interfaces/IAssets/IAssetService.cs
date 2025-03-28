using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Models.Users;

namespace SagaOrchestrator.Application.Orchestration.Interfaces.IAssets
{
    public interface IAssetService
    {
        Task<AssetDto> GetAssetByIdAsync(int assetId);
    }
}