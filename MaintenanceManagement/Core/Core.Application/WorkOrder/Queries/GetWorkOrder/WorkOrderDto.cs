using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderDto  : IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public int Id { get; set; }
        public int CompanyId { get; set; } 
        public int UnitId { get; set; } 
        public int? WorkOrderTypeId { get; set; }
        public string? WorkOrderTypeDesc { get; set; }         
        public string? RequestId { get; set; }
        public int RequestTypeId { get; set; }  
        public string? RequestTypeDesc { get; set; }  
        public int? MachineId { get; set; }
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }
        public string? MachineGroupName { get; set; }
        public int? MachineGroupId { get; set; }
        public int PriorityId { get; set; }        
        public string? PriorityDesc { get; set; } 
        public string? Remarks { get; set; }
        public string? Image { get; set; }
        public int StatusId { get; set; }          
        public DateTimeOffset LastActivityDate { get; set; }   
        public string? StatusDesc { get; set; } 
        public int? VendorId { get; set; } 
        public string? OldVendorId { get; set; } 
        public string? VendorName { get; set; }  
        public int RootCauseId { get; set; }                
        public string? RootCauseDesc { get; set; }    
    }
}
