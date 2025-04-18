using Core.Application.WorkOrder.Queries.GetWorkOrder;

namespace Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder
{
    public interface IWorkOrderCommandRepository
    {
        Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> CreateAsync(Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder, CancellationToken cancellationToken);
        Task<bool>  UpdateAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder);
        Task<bool>  DeleteAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder);     
        Task<bool> UpdateWOImageAsync(int workOrderId, string imageName);
        Task<WorkOrderDto?> GetByWOImageAsync(int workOrderId);
        Task<bool> RemoveWOImageReferenceAsync(int workOrderId);         
    }
}