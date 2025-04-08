using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrderMaster.WorkOrder
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
        public async Task<bool> DeleteAsync(int Id, Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder)
        {
            var workOrderToDelete = await _applicationDbContext.WorkOrder.FirstOrDefaultAsync(u => u.Id == Id);
            if (workOrderToDelete != null)
            {
                workOrderToDelete.IsDeleted = workOrder.IsDeleted;              
                return await _applicationDbContext.SaveChangesAsync()>0;
            }
            return false;
        }
        public async Task<int> UpdateAsync(Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder)
        {
            var existingDepGroup = await _applicationDbContext.WorkOrder.FirstOrDefaultAsync(u => u.Id == workOrder.Id);    
            if (existingDepGroup != null)
            {                
                existingDepGroup.WorkOrderTypeId = workOrder.WorkOrderTypeId;                
                existingDepGroup.RequestId = workOrder.RequestId;
                existingDepGroup.IsActive = workOrder.IsActive;
                existingDepGroup.PriorityId = workOrder.PriorityId;
                existingDepGroup.Remarks = workOrder.Remarks;
                existingDepGroup.StatusId = workOrder.StatusId;
                existingDepGroup.VendorId = workOrder.VendorId;
                existingDepGroup.Image = workOrder.Image;
                _applicationDbContext.WorkOrder.Update(existingDepGroup);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; 
        }                

        public async Task<bool> UpdateWOImageAsync(int workOrderId, string imageName)
        {
            var asset = await _applicationDbContext.WorkOrder.FindAsync(workOrderId);
            if (asset == null)
            {
                return false; 
            }            
            asset.Image = imageName.Replace(@"\", "/"); 

            asset.Image = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<WorkOrderDto?> GetByWOImageAsync(int workOrderId)
        {
            return await _applicationDbContext.WorkOrder
            .Where(a => a.Id == workOrderId)
            .Select(a => new WorkOrderDto
            {
                Id = a.Id,
                Image = a.Image                
            })
            .FirstOrDefaultAsync();
        }

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

        public async Task<string?> GetLatestRequestId(int CategoryId, CancellationToken cancellationToken)
        {
            var category = await _applicationDbContext.MaintenanceCategory
            .FirstOrDefaultAsync(x => x.Id == CategoryId, cancellationToken);

            if (category == null)
                throw new Exception("Invalid WorkOrderTypeId");

            string prefix = category.Description ?? "DIR";

             var maxId = await _applicationDbContext.WorkOrder
            .Where(x => x.WorkOrderTypeId == CategoryId)
            .MaxAsync(x => (int?)CategoryId, cancellationToken);

            int nextId = (maxId ?? 0) + 1;

            return $"{prefix}-{nextId}";
            
        }
    }
}