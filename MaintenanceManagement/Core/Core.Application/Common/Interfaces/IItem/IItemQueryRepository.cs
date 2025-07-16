using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IItem
{
    public interface IItemQueryRepository
    {
        Task<List<Core.Domain.Entities.ItemGroupCode>> GetGroupCodes(string OldUnitId);
        Task<List<Core.Domain.Entities.ItemMaster>> GetItemMasters(string OldUnitId,string Grpcode,string? ItemCode,string? ItemName);
    }
}