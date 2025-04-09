
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;

namespace Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder
{
    public interface IWorkOrderQueryRepository
    {         
        
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOPriorityDescAsync();  
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync();           
        Task<List<Core.Domain.Entities.MiscMaster>> GetWORequestTypeDescAsync();     
         Task<WorkOrderDto>  GetByIdAsync(int workOrderId);                     
        Task<(dynamic WorkOrderResult,  IEnumerable<dynamic> Activity, IEnumerable<dynamic> Schedule, IEnumerable<dynamic> Item,IEnumerable<dynamic> Technician )> GetWorkOrderByIdAsync(int workOrderId);        
    }
}
