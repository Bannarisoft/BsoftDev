using Core.Application.Common.Mappings;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderCombineDto :  IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public int? WorkOrderTypeId { get; set; }        
        public string? RequestId { get; set; }
        public int RequestTypeId { get; set; }  
        public int PriorityId { get; set; }     
        public string? MachineCode { get; set; }    
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }          
        public int? VendorId { get; set; } 
        public string? OldVendorId { get; set; } 
        public string? VendorName { get; set; }  
        public int RootCauseId { get; set; }         
        
        public ICollection<WorkOrderActivityDto>? WorkOrderActivity{ get; set; }       
        public ICollection<WorkOrderScheduleDto>? WorkOrderSchedule{ get; set; }       
        public ICollection<WorkOrderTechnicianDto>? WorkOrderTechnician{ get; set; } 
        public ICollection<WorkOrderItemDto>? WorkOrderItem{ get; set; } 
    }
}