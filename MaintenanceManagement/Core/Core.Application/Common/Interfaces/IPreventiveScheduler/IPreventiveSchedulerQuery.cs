using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPreventiveScheduler
{
    public interface IPreventiveSchedulerQuery
    {
        Task<(IEnumerable<dynamic> PreventiveSchedulerList, int)> GetAllPreventiveSchedulerAsync(int PageNumber, int PageSize, string? SearchTerm,List<int> departmentIds);
        Task<PreventiveSchedulerHeader> GetByIdAsync(int id);

        Task<bool> SoftDeleteValidation(int Id);
        Task<bool> AlreadyExistsAsync(int activityId, int machinegroupId, int? id = null);
        Task<bool> NotFoundAsync(int id);
        Task<bool> NotFoundDetailAsync(int id);
        Task<(DateTime nextDate, DateTime reminderDate)> CalculateNextScheduleDate(DateTime startDate, int interval, string unit, int reminderDays);
        Task<List<PreventiveSchedulerDetail>> GetPreventiveSchedulerDetail(int PreventiveSchedulerId);
        Task<DateTimeOffset?> GetLastMaintenanceDateAsync(int machineId, int PreventiveDetailId, string miscType, string misccode);
        Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailById(int Id);
        Task<bool> UpdateValidation(int id);
        Task<IEnumerable<dynamic>> GetAbstractSchedulerByDate(int DepartmentId);
        Task<IEnumerable<dynamic>> GetDetailSchedulerByDate(DateOnly schedulerDate, int DepartmentId);
        Task<PreventiveSchedulerHeader> GetWorkOrderScheduleDetailById(int Id);
        Task<bool> MachingroupValidation(int id);
        Task<bool> ExistWorkOrderBySchedulerDetailId(int id);
        Task<bool> ExistPreventivescheduleItem(int id);
        Task<PreventiveSchedulerHeader> GetWorkOrderScheduleDetailWithoutItemidById(int Id);
        Task<Core.Domain.Entities.MachineGroup> GetMachineGroupIdByName(string MachineGroupName);
        Task<Core.Domain.Entities.MachineMaster> GetMachineIdByCode(string MachineCode);
        Task<PreventiveSchedulerDetail> GetPreventiveSchedulerDetailByName(string PreventiveSchedulerName, string MachineCode);
        Task<Core.Domain.Entities.ActivityMaster> GetActivityIdByName(string ActivityName);
        Task<PreventiveSchedulerHeader> GetDetailSchedulerByPreventiveScheduleId(int Id);
        Task<List<Core.Domain.Entities.MachineMaster>> GetUnMappedMachineIdByCode(int Id);
    }
}