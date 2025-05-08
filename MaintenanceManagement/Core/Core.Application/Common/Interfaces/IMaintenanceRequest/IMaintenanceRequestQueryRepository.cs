using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.MaintenanceRequest.Queries.GetExternalRequestById;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.RequestReport;
using Core.Application.WorkOrder.Queries.GetRequestType;

namespace Core.Application.Common.Interfaces.IMaintenanceRequest
{
    public interface IMaintenanceRequestQueryRepository
    {

          Task<(IEnumerable<dynamic> MaintenanceRequestList, int)> GetAllMaintenanceRequestAsync(int PageNumber, int PageSize, string? SearchTerm);
          Task<(IEnumerable<dynamic> MaintenanceRequestList, int)> GetAllMaintenanceExternalRequestAsync(int PageNumber, int PageSize, string? SearchTerm);
        // Task<Core.Domain.Entities.MaintenanceRequest?> GetByIdAsync(int Id);
         Task<dynamic?> GetByIdAsync(int id);
         Task<List<GetExternalRequestByIdDto>> GetExternalRequestByIdAsync(List<int> ids);
       Task<List<Core.Domain.Entities.ExistingVendorDetails>> GetVendorDetails(string OldUnitId,string? VendorCode);   
         
          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenancestatusAsync();   
          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceOpenstatusAsync(); 
          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceRequestTypeAsync();

           Task<bool> GetWOclosedAsync(int id);
          Task<bool> GetWOclosedOrInProgressAsync(int id);
          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceStatusDescAsync();  

          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceServiceDescAsync(); 
           Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceServiceLocationDescAsync();  
          Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceSpareTypeDescAsync();
         Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceDispatchModeDescAsync();

          Task<List<RequestReportDto>> GetMaintenanceStatusDescAsync( DateTimeOffset? RequestFromDate, DateTimeOffset? RequestToDate , int? GetRequestType , int?  RequestStatus);  
          
           
          

          
          

    }
}