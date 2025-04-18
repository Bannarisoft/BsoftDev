using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class WorkOrderCombineDto :  IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {        
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public string? WorkOrderDocNo { get; set; }        
        public int RequestId { get; set; }                
        public int PreventiveScheduleId { get; set; }         
        public int StatusId { get; set; }     
        public int RootCauseId { get; set; }         
        public string? Remarks { get; set; }
        public string? Image { get; set; }           
        public int TotalManPower { get; set; }         
        public decimal TotalSpentHours { get; set; }         
        
        public ICollection<WorkOrderActivityDto>? WorkOrderActivity{ get; set; }       
        public ICollection<WorkOrderScheduleDto>? WorkOrderSchedule{ get; set; }       
        public ICollection<WorkOrderTechnicianDto>? WorkOrderTechnician{ get; set; } 
        public ICollection<WorkOrderItemDto>? WorkOrderItem{ get; set; } 
        public ICollection<WorkOrderCheckListDto>? WorkOrderCheckList{ get; set; } 
    }
}