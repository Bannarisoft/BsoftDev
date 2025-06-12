using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.Power.IPowerConsumption
{
    public interface IPowerConsumptionCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.Power.PowerConsumption powerConsumption);
    }
}