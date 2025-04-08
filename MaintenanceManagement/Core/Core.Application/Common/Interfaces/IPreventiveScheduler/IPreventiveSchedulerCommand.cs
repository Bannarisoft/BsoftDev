using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPreventiveScheduler
{
    public interface IPreventiveSchedulerCommand
    {
        Task<int> CreateAsync(PreventiveSchedulerHdr preventiveSchedulerHdr);     
        Task<bool> UpdateAsync(PreventiveSchedulerHdr preventiveSchedulerHdr);
        Task<bool> DeleteAsync(int id,PreventiveSchedulerHdr preventiveSchedulerHdr); 
    }
}