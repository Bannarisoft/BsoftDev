using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ILocation
{
    public interface ILocationCommandRepository
    {
        Task<Core.Domain.Entities.Location> CreateAsync(Core.Domain.Entities.Location location);     
        Task<bool> UpdateAsync(Core.Domain.Entities.Location location);
        Task<int> DeleteAsync(int id,Core.Domain.Entities.Location location); 
         Task<int> GetMaxSortOrderAsync();
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   


    }
}