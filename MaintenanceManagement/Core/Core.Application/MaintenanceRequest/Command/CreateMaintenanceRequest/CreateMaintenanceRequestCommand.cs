using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using MediatR;

namespace Core.Application.MaintenanceRequest.Command.CreateMaintenanceRequest
{
    public class CreateMaintenanceRequestCommand  : IRequest<ApiResponseDTO<GetMaintenanceRequestDto>>
    {
       public int RequestTypeId  { get; set; }       
       public int MaintenanceTypeId  { get; set; }     
       public int MachineId { get; set; }            
       public int DepartmentId { get; set; }
       public int SourceId { get; set; }
       public int? VendorId  { get; set; }
       public string? OldVendorId  { get; set; }
       public string? Remarks { get; set; }
       public string? RequestId { get; set; }
        
    }
}