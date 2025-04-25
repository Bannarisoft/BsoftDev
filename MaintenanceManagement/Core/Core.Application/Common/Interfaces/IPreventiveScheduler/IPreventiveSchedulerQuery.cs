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
        Task<bool> AlreadyExistsAsync(string ShiftName,int? id = null);
        Task<bool> NotFoundAsync(int id );
        Task<(DateTime nextDate, DateTime reminderDate)> CalculateNextScheduleDate(DateTime startDate, int interval, string unit,int reminderDays);
        Task<List<PreventiveSchedulerDetail>> GetPreventiveSchedulerDetail(int PreventiveSchedulerId);
        Task<DateTimeOffset?> GetLastMaintenanceDateAsync(int PreventiveSchedulerId);
        Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailById(int Id);
    }
}