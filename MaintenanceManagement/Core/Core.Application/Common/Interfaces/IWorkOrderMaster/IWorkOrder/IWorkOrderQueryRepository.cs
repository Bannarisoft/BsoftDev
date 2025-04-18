
using Core.Application.WorkOrder.Queries.GetWorkOrder;

namespace Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder
{
    public interface IWorkOrderQueryRepository
    {         
        
        Task<List<Core.Domain.Entities.MiscMaster>> GetWORootCauseDescAsync();  
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync();        
        Task<WorkOrderDto>  GetByIdAsync(int workOrderId);  
        Task<string> GetBaseDirectoryAsync();
        Task<(dynamic WorkOrderResult,  IEnumerable<dynamic> Activity, IEnumerable<dynamic> Item,IEnumerable<dynamic> Technician,IEnumerable<dynamic> checkList,IEnumerable<dynamic> schedule )> GetWorkOrderByIdAsync(int workOrderId);        
        Task<(List<WorkOrderDto>,int)> GetAllWOAsync(int PageNumber, int PageSize, string? SearchTerm);    
        Task<string?> GetLatestWorkOrderDocNo   ( string maintenanceType);
    }
}
