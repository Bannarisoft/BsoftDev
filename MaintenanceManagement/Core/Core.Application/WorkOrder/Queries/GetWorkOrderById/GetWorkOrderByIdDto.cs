
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdDto
    {   
        
        public string? WorkOrderDocNo { get; set; }                   
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }                         
        public string? StatusDesc { get; set; }
        public int RootCauseId { get; set; }  
        public string? RootCauseDesc { get; set; }
        

        public IList<GetWorkOrderActivityByIdDto>? WOActivity { get; set; }
        public IList<GetWorkOrderItemByIdDto>? WOItem { get; set; }        
        public IList<GetWorkOrderTechnicianByIdDto>? WOTechnician { get; set; }
        public IList<GetWorkOrderCheckListByIdDto>? WOCheckList { get; set; }
        public IList<GetWorkOrderScheduleByIdDto>? WOSchedule { get; set; }
    } 
}