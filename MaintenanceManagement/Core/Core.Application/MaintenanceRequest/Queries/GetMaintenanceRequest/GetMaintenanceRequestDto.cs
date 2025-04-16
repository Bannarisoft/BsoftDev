using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest
{
    public class GetMaintenanceRequestDto
    {  
         public int Id { get; set; }
      public string? RequestId { get; set; }
       public int RequestTypeId  { get; set; }  
       public string? RequestTypeName  { get; set; }   
       public int MaintenanceTypeId  { get; set; }    
       public string? MaintenanceTypeName { get; set; }  
       public int MachineId { get; set; }
       public string? MachineName { get; set; }
       public int DepartmentId { get; set; }
       public string? DepartmentName { get; set; }
       public int SourceId { get; set; }
       public int? VendorId  { get; set; }
       public string? VendorName  { get; set; } = null;
       public string? OldVendorId  { get; set; }

       public string?   OldVendorName  { get; set; }
       public string? Remarks { get; set; }

       
    }
} 