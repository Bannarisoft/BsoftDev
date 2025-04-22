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
         private  readonly IMaintenanceRequestQueryRepository _maintenanceRequestQueryRepository;
         
          public MaintenanceRequestCommandRepository(ApplicationDbContext applicationDbContext , IMaintenanceRequestQueryRepository maintenanceRequest)
        {
            _dbContext = applicationDbContext;
            _maintenanceRequestQueryRepository = maintenanceRequest;
        }      
        public async Task<int> CreateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest)
        {
             await _dbContext.MaintenanceRequest.AddAsync(maintenanceRequest);
               return await _dbContext.SaveChangesAsync();
              //  return maintenanceRequest.Id;
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
                    existingRequest.CompanyId = maintenanceRequest.CompanyId;
                    existingRequest.UnitId = maintenanceRequest.UnitId;
                    existingRequest.DepartmentId = maintenanceRequest.DepartmentId;
                    existingRequest.SourceId = maintenanceRequest.SourceId;
                    existingRequest.VendorId = maintenanceRequest.VendorId;
                    existingRequest.OldVendorId = maintenanceRequest.OldVendorId;
                    existingRequest.ServiceTypeId = maintenanceRequest.ServiceTypeId;
                    existingRequest.ServiceLocationId = maintenanceRequest.ServiceLocationId;
                    existingRequest.ModeOfDispatchId = maintenanceRequest.ModeOfDispatchId;
                    existingRequest.ExpectedDispatchDate = maintenanceRequest.ExpectedDispatchDate;
                    existingRequest.SparesTypeId = maintenanceRequest.SparesTypeId;
                    existingRequest.EstimatedServiceCost = maintenanceRequest.EstimatedServiceCost;
                    existingRequest.EstimatedSpareCost = maintenanceRequest.EstimatedSpareCost;
                    existingRequest.RequestStatusId = maintenanceRequest.RequestStatusId;
                    existingRequest.Remarks = maintenanceRequest.Remarks;
                    

                    _dbContext.MaintenanceRequest.Update(existingRequest);
                    return await _dbContext.SaveChangesAsync() > 0;
                }

                return false;
        }

  
        public async Task<bool> UpdateStatusAsync(int id)
        {
            // Step 1: Get the maintenance status from MiscMaster (e.g., "Closed")
            var statusList = await _maintenanceRequestQueryRepository.GetMaintenancestatusAsync();
            var status = statusList.FirstOrDefault();

            if (status == null)
                return false;

            // Step 2: Find the MaintenanceRequest by ID
            var entity = await _dbContext.MaintenanceRequest.FindAsync(id);
            if (entity == null)
                return false;

            // Step 3: Update with new status ID
            entity.RequestStatusId = status.Id;
            entity.ModifiedDate = DateTime.UtcNow;

            _dbContext.MaintenanceRequest.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }


    }
}