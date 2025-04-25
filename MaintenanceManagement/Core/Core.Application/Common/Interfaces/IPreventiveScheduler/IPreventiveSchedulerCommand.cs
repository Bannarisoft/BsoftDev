using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPreventiveScheduler
{
    public interface IPreventiveSchedulerCommand
    {
        Task<int> CreateAsync(PreventiveSchedulerHeader preventiveSchedulerHdr);     
        Task<PreventiveSchedulerHeader> UpdateAsync(PreventiveSchedulerHeader preventiveSchedulerHdr);
        Task<bool> DeleteAsync(int id,PreventiveSchedulerHeader preventiveSchedulerHdr);
        Task<PreventiveSchedulerDetail> CreateDetailAsync(PreventiveSchedulerDetail preventiveSchedulerDetail); 
        Task<bool> UpdateDetailAsync(int id,string HangfireJobId); 
        Task<bool> UpdateRescheduleDate(int id,DateOnly RescheduleDate);
        Task<bool> CreateNextSchedulerDetailAsync(int Id);      
    }
}