using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Power.PowerConsumption.Queries.GetClosingReaderValueById;

namespace Core.Application.Common.Interfaces.Power.IGeneratorConsumption
{
    public interface IGeneratorConsumptionQueryRepository
    {
        Task<GetClosingReaderValueDto> GetOpeningReaderValueById(int feederId);
    }
}