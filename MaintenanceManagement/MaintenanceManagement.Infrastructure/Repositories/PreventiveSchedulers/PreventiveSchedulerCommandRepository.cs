using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Core.Domain.Common.BaseEntity;

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

        public async Task<PreventiveSchedulerDetail> CreateDetailAsync(PreventiveSchedulerDetail preventiveSchedulerDetail)
        {
            await _applicationDbContext.PreventiveSchedulerDtl.AddRangeAsync(preventiveSchedulerDetail);
             await _applicationDbContext.SaveChangesAsync(); 
             return preventiveSchedulerDetail;
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

        public async Task<PreventiveSchedulerHeader> UpdateAsync(PreventiveSchedulerHeader preventiveSchedulerHdr)
        {
            var existingPreventiveScheduler = await _applicationDbContext.PreventiveSchedulerHdr
            .Include(ps => ps.PreventiveSchedulerDetails)
            .Include(ps => ps.PreventiveSchedulerActivities)
            .Include(ps => ps.PreventiveSchedulerItems)
            .FirstOrDefaultAsync(ps => ps.Id == preventiveSchedulerHdr.Id);

            
            if (existingPreventiveScheduler != null)
            {

                   
               _applicationDbContext.PreventiveSchedulerActivity.RemoveRange(
                   _applicationDbContext.PreventiveSchedulerActivity.Where(x => x.PreventiveSchedulerHeaderId == preventiveSchedulerHdr.Id));

               _applicationDbContext.PreventiveSchedulerItems.RemoveRange(
                   _applicationDbContext.PreventiveSchedulerItems.Where(x => x.PreventiveSchedulerHeaderId == preventiveSchedulerHdr.Id));

                existingPreventiveScheduler.MachineGroupId = preventiveSchedulerHdr.MachineGroupId;
                existingPreventiveScheduler.DepartmentId = preventiveSchedulerHdr.DepartmentId;
                existingPreventiveScheduler.MaintenanceCategoryId = preventiveSchedulerHdr.MaintenanceCategoryId;
                existingPreventiveScheduler.ScheduleId = preventiveSchedulerHdr.ScheduleId;
                existingPreventiveScheduler.FrequencyTypeId = preventiveSchedulerHdr.FrequencyTypeId;
                existingPreventiveScheduler.FrequencyInterval = preventiveSchedulerHdr.FrequencyInterval;
                existingPreventiveScheduler.EffectiveDate = preventiveSchedulerHdr.EffectiveDate;
                existingPreventiveScheduler.FrequencyUnitId = preventiveSchedulerHdr.FrequencyUnitId;
                existingPreventiveScheduler.EffectiveDate = preventiveSchedulerHdr.EffectiveDate;
                existingPreventiveScheduler.GraceDays = preventiveSchedulerHdr.GraceDays;
                existingPreventiveScheduler.ReminderWorkOrderDays = preventiveSchedulerHdr.ReminderWorkOrderDays;
                existingPreventiveScheduler.ReminderMaterialReqDays = preventiveSchedulerHdr.ReminderMaterialReqDays;
                existingPreventiveScheduler.IsDownTimeRequired = preventiveSchedulerHdr.IsDownTimeRequired;
                existingPreventiveScheduler.DownTimeEstimateHrs = preventiveSchedulerHdr.DownTimeEstimateHrs;
                existingPreventiveScheduler.IsDeleted = preventiveSchedulerHdr.IsDeleted;
                existingPreventiveScheduler.IsActive = preventiveSchedulerHdr.IsActive;
                _applicationDbContext.PreventiveSchedulerHdr.Update(existingPreventiveScheduler);

               if (preventiveSchedulerHdr.PreventiveSchedulerActivities?.Any() == true)
                   await _applicationDbContext.PreventiveSchedulerActivity.AddRangeAsync(preventiveSchedulerHdr.PreventiveSchedulerActivities);

               if (preventiveSchedulerHdr.PreventiveSchedulerItems?.Any() == true)
                   await _applicationDbContext.PreventiveSchedulerItems.AddRangeAsync(preventiveSchedulerHdr.PreventiveSchedulerItems);

                    if (preventiveSchedulerHdr.PreventiveSchedulerDetails?.Any() == true)
                  {
                      foreach (var incomingDetail in preventiveSchedulerHdr.PreventiveSchedulerDetails)
                      {
                          var PreventiveSchedulerDetail = await _applicationDbContext.PreventiveSchedulerDtl
                              .FirstOrDefaultAsync(d => d.Id == incomingDetail.Id);

                          if (PreventiveSchedulerDetail != null)
                          {
                              
                              PreventiveSchedulerDetail.IsActive = incomingDetail.IsActive;
                              PreventiveSchedulerDetail.WorkOrderCreationStartDate = incomingDetail.WorkOrderCreationStartDate;
                              PreventiveSchedulerDetail.ActualWorkOrderDate = incomingDetail.ActualWorkOrderDate;
                              PreventiveSchedulerDetail.MaterialReqStartDays = incomingDetail.MaterialReqStartDays;
                          }
                      }
                  }

                 await _applicationDbContext.SaveChangesAsync() ;
                 return preventiveSchedulerHdr;
            }
            
            return preventiveSchedulerHdr; 
        }

        public async Task<bool> UpdateDetailAsync(int id,string HangfireJobId)
        {
             var existingPreventiveScheduler = await _applicationDbContext.PreventiveSchedulerDtl.FirstOrDefaultAsync(u => u.Id == id);

            if (existingPreventiveScheduler != null)
            {
                existingPreventiveScheduler.HangfireJobId = HangfireJobId;
                 _applicationDbContext.PreventiveSchedulerDtl.Update(existingPreventiveScheduler);
                return await _applicationDbContext.SaveChangesAsync() >0;
            }
            return false;
       
        }
    }
}