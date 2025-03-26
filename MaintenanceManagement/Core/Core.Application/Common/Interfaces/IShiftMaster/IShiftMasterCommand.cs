using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IShiftMaster
{
    public interface IShiftMasterCommand
    {
        Task<int> CreateAsync(ShiftMaster shiftMaster);     
        Task<bool> UpdateAsync(ShiftMaster shiftMaster);
        Task<bool> DeleteAsync(int id,ShiftMaster shiftMaster); 
    }
}