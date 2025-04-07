
namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderByIdDto
    {
        public int Id { get; set; }
        public int? WorkOrderTypeId { get; set; }        
        public string? WorkOrderTypeDesc { get; set; }        
        public int RequestId { get; set; }
        public int PriorityId { get; set; }         
        public string? PriorityDesc { get; set; }
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }                         
        public string? StatusDesc { get; set; }
        public string? VendorId { get; set; }   
        public string? VendorDesc { get; set; }   
        public int RootCauseId { get; set; }  
        public int RootCauseDesc { get; set; }  

        public IList<GetWorkOrderActivityByIdDto>? WorkOrderActivity { get; set; }
        public IList<GetWorkOrderItemByIdDto>? WorkOrderItem { get; set; }
        public IList<GetWorkOrderScheduleByIdDto>? WorkOrderSchedule { get; set; }
        public IList<GetWorkOrderTechnicianByIdDto>? WorkOrderTechnician { get; set; }
    } 
}