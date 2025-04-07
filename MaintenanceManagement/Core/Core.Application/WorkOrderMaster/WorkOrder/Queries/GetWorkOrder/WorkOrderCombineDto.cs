using Core.Application.Common.Mappings;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderCombineDto :  IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public int? WorkOrderTypeId { get; set; }        
        public string? RequestId { get; set; }
        public int PriorityId { get; set; }         
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }          
        public string? VendorId { get; set; }   

        public WorkOrderActivityDto?  AssetLocation  { get; set; }
        public ICollection<WorkOrderActivityDto>? WorkOrderActivity{ get; set; }       
        public ICollection<WorkOrderScheduleDto>? WorkOrderSchedule{ get; set; }       
        public ICollection<WorkOrderTechnicianDto>? WorkOrderTechnician{ get; set; } 
        public ICollection<WorkOrderItemDto>? WorkOrderItem{ get; set; } 
    }
}