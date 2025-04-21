using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;

// using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrder
{
    public class WorkOrderCommandRepository: IWorkOrderCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;        
        public WorkOrderCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;            
        }
        public async Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> CreateAsync(Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder, CancellationToken cancellationToken)
        {
            var entry =_applicationDbContext.Entry(workOrder);
            await _applicationDbContext.WorkOrder.AddAsync(workOrder);
            await _applicationDbContext.SaveChangesAsync();
            return workOrder;   
        }      
        public async Task<bool> UpdateAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder)
        {
             var existingWorkOrder = await _applicationDbContext.WorkOrder
                   .Include(cf => cf.WorkOrderItems)
                   .Include(cf => cf.WorkOrderActivities)
                   .Include(cf => cf.WorkOrderSchedules)
                   .Include(cf => cf.WorkOrderTechnicians)
                   .Include(cf => cf.WorkOrderCheckLists)
                   .FirstOrDefaultAsync(u => u.Id ==workOrderId);

               if (existingWorkOrder == null)
                   return false;
               
               _applicationDbContext.WorkOrderActivity.RemoveRange(
                   _applicationDbContext.WorkOrderActivity.Where(x => x.WorkOrderId == workOrderId));

               _applicationDbContext.WorkOrderCheckList.RemoveRange(
                   _applicationDbContext.WorkOrderCheckList.Where(x => x.WorkOrderId == workOrderId));

               _applicationDbContext.WorkOrderItem.RemoveRange(
                   _applicationDbContext.WorkOrderItem.Where(x => x.WorkOrderId == workOrderId));                
           
                _applicationDbContext.WorkOrderTechnician.RemoveRange(
                   _applicationDbContext.WorkOrderTechnician.Where(x => x.WorkOrderId == workOrderId));

                       
                existingWorkOrder. StatusId = workOrder.StatusId;                
                existingWorkOrder.RootCauseId = workOrder.RootCauseId;                
                existingWorkOrder.Remarks = workOrder.Remarks;
                existingWorkOrder.Image = workOrder.Image;
                existingWorkOrder.TotalManPower = workOrder.TotalManPower;
                existingWorkOrder.TotalSpentHours = workOrder.TotalSpentHours;

                if (workOrder.WorkOrderActivities?.Any() == true)
                   await _applicationDbContext.WorkOrderActivity.AddRangeAsync(workOrder.WorkOrderActivities);
                if (workOrder.WorkOrderItems?.Any() == true)
                   await _applicationDbContext.WorkOrderItem.AddRangeAsync(workOrder.WorkOrderItems);              
                if (workOrder.WorkOrderTechnicians?.Any() == true)
                   await _applicationDbContext.WorkOrderTechnician.AddRangeAsync(workOrder.WorkOrderTechnicians);
                if (workOrder.WorkOrderCheckLists?.Any() == true)
                   await _applicationDbContext.WorkOrderCheckList.AddRangeAsync(workOrder.WorkOrderCheckLists);             

               
               return await _applicationDbContext.SaveChangesAsync() > 0;
        }                

        public async Task<bool> UpdateWOImageAsync(int workOrderId, string imageName)
        {
            var imageExists = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (imageExists == null)
            {
                return false; 
            }            
            imageExists.Image = imageName.Replace(@"\", "/"); 

            imageExists.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        // public async Task<WorkOrderDto?> GetByWOImageAsync(int workOrderId)
        // {
        //     return await _applicationDbContext.WorkOrder
        //     .Where(a => a.Id == workOrderId)
        //     .Select(a => new WorkOrderDto
        //     {
        //         Id = a.Id,
        //         Image = a.Image                
        //     })
        //     .FirstOrDefaultAsync();
        // }

        public async Task<bool> RemoveWOImageReferenceAsync(int workOrderId)
        {
            var asset = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (asset == null)
            {
                return false;  // Asset not found
            }

            asset.Image = null;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreateScheduleAsync(int workOrderId, Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule)
        {           
       
            await _applicationDbContext.WorkOrderSchedule.AddAsync(workOrderSchedule);
            await _applicationDbContext.SaveChangesAsync();
            return workOrderSchedule.Id;   
                    
        }
        public async Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> GetByIdAsync(int workOrderId)
        {
           return await _applicationDbContext.WorkOrder
                     .FirstOrDefaultAsync(x => x.Id == workOrderId);
        }
    }
}