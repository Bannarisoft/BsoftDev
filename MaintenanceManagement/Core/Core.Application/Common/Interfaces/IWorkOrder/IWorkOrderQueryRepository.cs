
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;

namespace Core.Application.Common.Interfaces.IWorkOrder
{
    public interface IWorkOrderQueryRepository
    {         
        
        Task<List<Core.Domain.Entities.MiscMaster>> GetWORootCauseDescAsync();  
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync();        
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOSourceDescAsync();              
        Task<List<Core.Domain.Entities.MiscMaster>> GetWOStoreTypeDescAsync();     
        Task<string> GetBaseDirectoryAsync();                
        Task<(dynamic WorkOrderResult,  IEnumerable<dynamic> Activity, IEnumerable<dynamic> Item,IEnumerable<dynamic> Technician,IEnumerable<dynamic> checkList,IEnumerable<dynamic> schedule )> GetWorkOrderByIdAsync(int workOrderId);        
        Task<(List<WorkOrderWithScheduleDto>,int)> GetAllWOAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate,string? requestType, int PageNumber, int PageSize, string? SearchTerm);            
    }
}
