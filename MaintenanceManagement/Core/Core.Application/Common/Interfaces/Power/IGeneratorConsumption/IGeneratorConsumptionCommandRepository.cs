using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.Power.IGeneratorConsumption
{
    public interface IGeneratorConsumptionCommandRepository
    {
         Task<int> CreateAsync(Core.Domain.Entities.Power.GeneratorConsumption generatorConsumption);
    }
}