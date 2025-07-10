using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById;


namespace Core.Application.Common.Interfaces.Power.IGeneratorConsumption
{
    public interface IGeneratorConsumptionQueryRepository
    {
        Task<GetClosingEnergyReaderValueDto> GetOpeningReaderValueById(int generatorId);
    }
}