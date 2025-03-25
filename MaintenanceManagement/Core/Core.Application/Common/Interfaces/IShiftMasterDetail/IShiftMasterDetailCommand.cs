using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IShiftMasterDetail
{
    public interface IShiftMasterDetailCommand
    {
         Task<int> CreateAsync(ShiftMasterDetail shiftMasterDetail);     
        Task<bool> UpdateAsync(ShiftMasterDetail shiftMasterDetail);
        Task<bool> DeleteAsync(int id,ShiftMasterDetail shiftMasterDetail); 
    }
}