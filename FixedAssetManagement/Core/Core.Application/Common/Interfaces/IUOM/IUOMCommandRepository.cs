using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IUOM
{
    public interface IUOMCommandRepository
    {
        Task<Core.Domain.Entities.UOM> CreateAsync(Core.Domain.Entities.UOM uom);     
        Task<bool> UpdateAsync(Core.Domain.Entities.UOM location);
        Task<bool> DeleteAsync(int id, Core.Domain.Entities.UOM uom); 
         Task<int> GetMaxSortOrderAsync();
         Task<(bool IsNameDuplicate, bool IsSortOrderDuplicate)> CheckForDuplicatesAsync(string name, int sortOrder, int excludeId);   


    }
}