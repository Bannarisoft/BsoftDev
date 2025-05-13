using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contracts.Dtos.Maintenance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.External.IUnit;

namespace MaintenanceManagement.Infrastructure.HttpClients.Units
{
    public class UnitService : IUnitService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UnitService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<UnitDto>> GetUnitAutoCompleteAsync()
        {
            var client = _httpClientFactory.CreateClient("UnitClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "api/Unit/by-name");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<List<UnitDto>>>();
                return result?.Data ?? new List<UnitDto>();
            }

            // Optional: handle failure
            return new List<UnitDto>();
        }
        


    }
}