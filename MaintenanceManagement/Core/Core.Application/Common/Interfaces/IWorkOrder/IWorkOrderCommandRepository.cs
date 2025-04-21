using Core.Application.WorkOrder.Queries.GetWorkOrderById;

namespace Core.Application.Common.Interfaces.IWorkOrder
{
    public interface IWorkOrderCommandRepository
    {
        Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> CreateAsync(Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder, CancellationToken cancellationToken);
        Task<bool>  UpdateAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder);         
        Task<int>  CreateScheduleAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule);   
        Task<bool> UpdateWOImageAsync(int workOrderId, string imageName);
        //Task<GetWorkOrderByIdDto?> GetByWOImageAsync(int workOrderId);
        Task<bool> RemoveWOImageReferenceAsync(int workOrderId);         
        Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder>  GetByIdAsync(int workOrderId);  
    }
}