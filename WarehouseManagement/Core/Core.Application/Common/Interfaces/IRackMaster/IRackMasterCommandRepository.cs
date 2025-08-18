using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IRackMaster
{
    public interface IRackMasterCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.RackMaster rackMaster);

        Task<Core.Domain.Entities.RackMaster?> GetByIdAsync(int id);
        Task<int> UpdateAsync(Core.Domain.Entities.RackMaster rackMaster);
         
        Task<bool> DeleteAsync(int id, Core.Domain.Entities.RackMaster rackMaster);


    }
}