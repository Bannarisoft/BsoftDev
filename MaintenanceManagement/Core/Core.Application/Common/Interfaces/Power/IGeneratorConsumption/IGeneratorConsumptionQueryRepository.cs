using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId;
using Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById;
using Core.Application.Power.GeneratorConsumption.Queries.GetGeneratorConsumption;
using Core.Application.Power.GeneratorConsumption.Queries.GetUnitIdBasedOnMachineId;


namespace Core.Application.Common.Interfaces.Power.IGeneratorConsumption
{
    public interface IGeneratorConsumptionQueryRepository
    {
        Task<GetClosingEnergyReaderValueDto> GetOpeningReaderValueById(int generatorId);
        Task<List<GetMachineIdBasedonUnitDto>> GetMachineIdBasedonUnit();
        Task<(List<GetGeneratorConsumptionDto>, int)> GetAllGeneratorConsumptionAsync(int PageNumber, int PageSize, string? SearchTerm);
        
    }
}