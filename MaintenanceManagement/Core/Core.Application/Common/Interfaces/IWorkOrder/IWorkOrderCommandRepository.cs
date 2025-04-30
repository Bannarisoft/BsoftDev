
namespace Core.Application.Common.Interfaces.IWorkOrder
{
    public interface IWorkOrderCommandRepository
    {

        public Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder> CreateAsync( Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder, int requestTypeId, CancellationToken cancellationToken);
        public Task<bool>  UpdateAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrder workOrder);         
        Task<int>  CreateScheduleAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule);   
        Task<bool>  UpdateScheduleAsync(int workOrderId,Core.Domain.Entities.WorkOrderMaster.WorkOrderSchedule workOrderSchedule); 
        Task<bool> UpdateWOImageAsync(int workOrderId, string imageName);        
        Task<bool> DeleteWOImageAsync(string imageName);     
        Task<bool> DeleteItemImageAsync(string imageName);     
        Task<bool> UpdateWOItemImageAsync(int workOrderId, string imageName);   
        Task<bool> RemoveWOImageReferenceAsync(int workOrderId);         
        Task<Core.Domain.Entities.WorkOrderMaster.WorkOrder>  GetByIdAsync(int workOrderId);  
        Task<string?> GetLatestWorkOrderDocNo   (int TypeId); 
        Task<string> GetBaseDirectoryItemAsync();      
        Task<(string CompanyName, string UnitName)> GetCompanyUnitAsync(int companyId,int unitId);
        Task<Core.Domain.Entities.MiscMaster> GetMiscMasterByCodeAsync(string code); 
        Task<bool> RevertWorkOrderStatusAsync(int workOrderId);
    }
}