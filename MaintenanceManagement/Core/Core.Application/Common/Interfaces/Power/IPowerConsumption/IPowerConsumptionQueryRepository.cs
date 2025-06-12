using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Power.PowerConsumption.Queries.GetClosingReaderValueById;
using Core.Application.Power.PowerConsumption.Queries.GetFeederSubFeederById;
using Core.Application.Power.PowerConsumption.Queries.GetPowerConsumption;

namespace Core.Application.Common.Interfaces.Power.IPowerConsumption
{
    public interface IPowerConsumptionQueryRepository
    {
        Task<List<GetFeederSubFeederDto>> GetFeederSubFeedersById(int feederTypeId);
        Task<GetClosingReaderValueDto> GetOpeningReaderValueById(int feederId);
        Task<(List<GetPowerConsumptionDto>, int)> GetAllPowerConsumptionAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<GetPowerConsumptionDto> GetPowerConsumptionById(int id);

    }
}