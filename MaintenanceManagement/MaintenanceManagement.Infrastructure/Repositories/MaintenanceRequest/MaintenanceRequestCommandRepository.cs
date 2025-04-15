using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Repositories.MaintenanceRequest
{
    public class MaintenanceRequestCommandRepository    : IMaintenanceRequestCommandRepository
    {

         private readonly ApplicationDbContext _dbContext;
         
          public MaintenanceRequestCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }      
        public async Task<Core.Domain.Entities.MaintenanceRequest> CreateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest)
        {
             await _dbContext.MaintenanceRequest.AddAsync(maintenanceRequest);
                await _dbContext.SaveChangesAsync();
                return maintenanceRequest;
        }
         
         public async Task<bool> UpdateAsync( Core.Domain.Entities.MaintenanceRequest maintenanceRequest)
        {
                    var existingRequest = await _dbContext.MaintenanceRequest
                    .FirstOrDefaultAsync(m => m.Id == maintenanceRequest.Id);

                if (existingRequest != null)
                {
                    existingRequest.RequestTypeId = maintenanceRequest.RequestTypeId;
                    existingRequest.MaintenanceTypeId = maintenanceRequest.MaintenanceTypeId;
                    existingRequest.MachineId = maintenanceRequest.MachineId;
                    existingRequest.DepartmentId = maintenanceRequest.DepartmentId;
                    // existingRequest.SourceId = maintenanceRequest.SourceId;
                    // existingRequest.VendorId = maintenanceRequest.VendorId;
                    // existingRequest.OldVendorId = maintenanceRequest.OldVendorId;
                    existingRequest.Remarks = maintenanceRequest.Remarks;
                    existingRequest.IsActive = maintenanceRequest.IsActive;

                    _dbContext.MaintenanceRequest.Update(existingRequest);
                    return await _dbContext.SaveChangesAsync() > 0;
                }

                return false;
        }



    }
}