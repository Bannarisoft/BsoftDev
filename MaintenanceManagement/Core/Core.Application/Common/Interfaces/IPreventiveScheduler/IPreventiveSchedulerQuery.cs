using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPreventiveScheduler
{
    public interface IPreventiveSchedulerQuery
    {
        Task<(IEnumerable<dynamic> PreventiveSchedulerList,int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<PreventiveSchedulerHeader> GetByIdAsync(int id);
        Task<List<PreventiveSchedulerHeader>> GetPreventiveScheduler(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id); 
        Task<bool> AlreadyExistsAsync(int activityId,int machinegroupId,int? id = null);
        Task<bool> NotFoundAsync(int id );
        Task<(DateTime nextDate, DateTime reminderDate)> CalculateNextScheduleDate(DateTime startDate, int interval, string unit,int reminderDays);
        Task<List<PreventiveSchedulerDetail>> GetPreventiveSchedulerDetail(int PreventiveSchedulerId);
        Task<DateTimeOffset?> GetLastMaintenanceDateAsync(int machineId,int PreventiveSchedulerId,string miscType,string misccode);
        Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailById(int Id);
        Task<bool> UpdateValidation(int id);
        Task<IEnumerable<dynamic>> GetAbstractSchedulerByDate();
        Task<IEnumerable<dynamic>> GetDetailSchedulerByDate(DateOnly schedulerDate);
    }
}