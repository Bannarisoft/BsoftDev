
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdDto
    {
        public int Id { get; set; }
        public string? WorkOrderDocNo { get; set; }             
        public int RequestId { get; set; }
        public int PreventiveScheduleId { get; set; }      
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }                         
        public string? StatusDesc { get; set; }
        public string? VendorId { get; set; }   
        public string? VendorName { get; set; }   
        public int RootCauseId { get; set; }  
        public int RootCauseDesc { get; set; }
        public int TotalManPower { get; set; }         
        public decimal TotalSpentHours { get; set; }   
        

        public IList<GetWorkOrderActivityByIdDto>? WorkOrderActivity { get; set; }
        public IList<GetWorkOrderItemByIdDto>? WorkOrderItem { get; set; }
        public IList<GetWorkOrderScheduleByIdDto>? WorkOrderSchedule { get; set; }
        public IList<GetWorkOrderTechnicianByIdDto>? WorkOrderTechnician { get; set; }
        public IList<GetWorkOrderCheckListByIdDto>? WorkOrderCheckList { get; set; }
    } 
}