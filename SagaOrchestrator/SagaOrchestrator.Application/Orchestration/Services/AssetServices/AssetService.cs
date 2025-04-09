using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Models.Users;
using SagaOrchestrator.Application.Orchestration.Interfaces.IAssets;

namespace SagaOrchestrator.Application.Orchestration.Services.AssetServices
{
    public class AssetService : IAssetService
    {
        private readonly HttpClient _httpClient;
        public AssetService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<AssetDto> GetAssetByIdAsync(int assetId)
        {
            var response = await _httpClient.GetAsync($"/api/AssetMasterGeneral/{assetId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AssetDto>(content);
        }
    }
}