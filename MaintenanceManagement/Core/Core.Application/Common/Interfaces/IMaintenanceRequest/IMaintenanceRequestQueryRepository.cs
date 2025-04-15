using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;

namespace Core.Application.Common.Interfaces.IMaintenanceRequest
{
    public interface IMaintenanceRequestQueryRepository
    {

          Task<(List<Core.Domain.Entities.MaintenanceRequest>,int)> GetAllMaintenanceRequestAsync(int PageNumber, int PageSize, string? SearchTerm);
          Task<Core.Domain.Entities.MaintenanceRequest?> GetByIdAsync(int Id);
           Task<List<Core.Domain.Entities.ExistingVendorDetails>> GetVendorDetails(string OldUnitId,string? VendorCode);

    }
}