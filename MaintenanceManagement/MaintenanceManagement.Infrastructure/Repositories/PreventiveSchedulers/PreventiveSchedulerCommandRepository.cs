using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Repositories.PreventiveSchedulers
{
    public class PreventiveSchedulerCommandRepository : IPreventiveSchedulerCommand
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public PreventiveSchedulerCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> CreateAsync(PreventiveSchedulerHeader preventiveSchedulerHdr)
        {
            _applicationDbContext.Entry(preventiveSchedulerHdr);
            await _applicationDbContext.PreventiveSchedulerHdr.AddAsync(preventiveSchedulerHdr);
            await _applicationDbContext.SaveChangesAsync();

            return preventiveSchedulerHdr.Id;
        }

        public async Task<bool> DeleteAsync(int id, PreventiveSchedulerHeader preventiveSchedulerHdr)
        {
             var PreventiveSchedulerToDelete = await _applicationDbContext.PreventiveSchedulerHdr.FirstOrDefaultAsync(u => u.Id == id);
            if (PreventiveSchedulerToDelete != null)
            {
                PreventiveSchedulerToDelete.IsDeleted = preventiveSchedulerHdr.IsDeleted;
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false; 
        }

        public async Task<bool> UpdateAsync(PreventiveSchedulerHeader preventiveSchedulerHdr)
        {
            var existingPreventiveScheduler = await _applicationDbContext.PreventiveSchedulerHdr
            .Include(ps => ps.PreventiveSchedulerDetails)
            .Include(ps => ps.PreventiveSchedulerActivities)
            .Include(ps => ps.PreventiveSchedulerItems)
            .FirstOrDefaultAsync(ps => ps.Id == preventiveSchedulerHdr.Id);
            
            if (existingPreventiveScheduler != null)
            {
                 _applicationDbContext.PreventiveSchedulerDtl.RemoveRange(
                   _applicationDbContext.PreventiveSchedulerDtl.Where(x => x.PreventiveSchedulerId == preventiveSchedulerHdr.Id));

               _applicationDbContext.PreventiveSchedulerActivity.RemoveRange(
                   _applicationDbContext.PreventiveSchedulerActivity.Where(x => x.PreventiveSchedulerHdrId == preventiveSchedulerHdr.Id));

               _applicationDbContext.PreventiveSchedulerItems.RemoveRange(
                   _applicationDbContext.PreventiveSchedulerItems.Where(x => x.PreventiveSchedulerHdrId == preventiveSchedulerHdr.Id));

                existingPreventiveScheduler.MachineGroupId = preventiveSchedulerHdr.MachineGroupId;
                existingPreventiveScheduler.DepartmentId = preventiveSchedulerHdr.DepartmentId;
                existingPreventiveScheduler.MaintenanceCategoryId = preventiveSchedulerHdr.MaintenanceCategoryId;
                existingPreventiveScheduler.ScheduleId = preventiveSchedulerHdr.ScheduleId;
                existingPreventiveScheduler.DueTypeId = preventiveSchedulerHdr.DueTypeId;
                existingPreventiveScheduler.DuePeriod = preventiveSchedulerHdr.DuePeriod;
                existingPreventiveScheduler.EffectiveDate = preventiveSchedulerHdr.EffectiveDate;
                existingPreventiveScheduler.FrequencyId = preventiveSchedulerHdr.FrequencyId;
                existingPreventiveScheduler.EffectiveDate = preventiveSchedulerHdr.EffectiveDate;
                existingPreventiveScheduler.GraceDays = preventiveSchedulerHdr.GraceDays;
                existingPreventiveScheduler.ReminderWorkOrderDays = preventiveSchedulerHdr.ReminderWorkOrderDays;
                existingPreventiveScheduler.ReminderMaterialReqDays = preventiveSchedulerHdr.ReminderMaterialReqDays;
                existingPreventiveScheduler.IsDownTimeRequired = preventiveSchedulerHdr.IsDownTimeRequired;
                existingPreventiveScheduler.DownTimeEstimateHrs = preventiveSchedulerHdr.DownTimeEstimateHrs;
                existingPreventiveScheduler.IsDeleted = preventiveSchedulerHdr.IsDeleted;
                existingPreventiveScheduler.IsActive = preventiveSchedulerHdr.IsActive;
                _applicationDbContext.PreventiveSchedulerHdr.Update(existingPreventiveScheduler);

                 if (preventiveSchedulerHdr.PreventiveSchedulerDetails?.Any() == true)
                   await _applicationDbContext.PreventiveSchedulerDtl.AddRangeAsync(preventiveSchedulerHdr.PreventiveSchedulerDetails);

               if (preventiveSchedulerHdr.PreventiveSchedulerActivities?.Any() == true)
                   await _applicationDbContext.PreventiveSchedulerActivity.AddRangeAsync(preventiveSchedulerHdr.PreventiveSchedulerActivities);

               if (preventiveSchedulerHdr.PreventiveSchedulerItems?.Any() == true)
                   await _applicationDbContext.PreventiveSchedulerItems.AddRangeAsync(preventiveSchedulerHdr.PreventiveSchedulerItems);

                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            
            return false; 
        }
    }
}