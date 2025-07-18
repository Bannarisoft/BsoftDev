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
        Task<List<PreventiveSchedulerDetail>> UpdateScheduleDetails(int HeaderId, List<PreventiveSchedulerDetail> preventiveSchedulerDetail);
        Task<bool> DeleteAsync(int id, PreventiveSchedulerHeader preventiveSchedulerHdr);
        Task<PreventiveSchedulerDetail> CreateDetailAsync(PreventiveSchedulerDetail preventiveSchedulerDetail);
        Task<bool> UpdateDetailAsync(int id, string HangfireJobId);
        Task<bool> UpdateRescheduleDate(int id, DateOnly RescheduleDate);
        Task<bool> CreateNextSchedulerDetailAsync(int Id);
        Task<bool> ScheduleInActive(PreventiveSchedulerHeader preventiveSchedulerHdr);
        Task<bool> DeleteDetailAsync(int id);
        Task<PreventiveSchedulerDetail> AddReScheduleDetailAsync(int Id, DateOnly RescheduleDate, CancellationToken cancellationToken);
        Task<PreventiveSchedulerDetail?> GetDetailByMachineActivityAndUnitAsync(string machineCode, string activityName, int unitId);
        Task<List<PreventiveSchedulerHeader>> BulkImportPreventiveHeaderAsync(List<PreventiveSchedulerHeader> preventiveSchedulerHeaders);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<PreventiveSchedulerHeader> UpdateScheduleMetadata(PreventiveSchedulerHeader preventiveSchedulerHdr);
        Task<PreventiveSchedulerDetail> UpdateScheduleDetails(PreventiveSchedulerDetail preventiveSchedulerDetail);
        
    }
}