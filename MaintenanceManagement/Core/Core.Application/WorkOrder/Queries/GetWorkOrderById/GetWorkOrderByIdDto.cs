
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdDto
    {   
        public int Id { get; set; }       
        public string? WorkOrderDocNo { get; set; }                   
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }                         
        public string? StatusDesc { get; set; }
        public int RootCauseId { get; set; }  
        public string? RootCauseDesc { get; set; }
        

        public IList<GetWorkOrderActivityByIdDto>? WorkOrderActivity { get; set; }
        public IList<GetWorkOrderItemByIdDto>? WorkOrderItem { get; set; }        
        public IList<GetWorkOrderTechnicianByIdDto>? WorkOrderTechnician { get; set; }
        public IList<GetWorkOrderCheckListByIdDto>? WorkOrderCheckList { get; set; }
        public IList<GetWorkOrderScheduleByIdDto>? WorkOrderSchedule { get; set; }
    } 
}