using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderDto  : IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
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
        public int RootCauseId { get; set; }                
    }
}