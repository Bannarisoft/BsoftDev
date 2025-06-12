using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.Power.IFeeder
{
    public interface IFeederCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.Power.Feeder feeder);
        Task<bool> UpdateAsync(int id, Core.Domain.Entities.Power.Feeder feeder);
        Task<bool> DeleteAsync(int id,Core.Domain.Entities.Power.Feeder feeder); 
    }
}