using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.IMachineSpecification;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Core.Domain.Common.BaseEntity;

namespace MaintenanceManagement.Infrastructure.Repositories.MachineSpecification
{
    public class MachineSpecificationCommandRepository: IMachineSpecificationCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MachineSpecificationCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

       public async Task<int> CreateAsync(Core.Domain.Entities.MachineSpecification machineSpecification)
        {
            await _applicationDbContext.MachineSpecification.AddAsync(machineSpecification);
            await _applicationDbContext.SaveChangesAsync();
            return machineSpecification.Id;
        }

        public async Task<int> DeleteAsync(int Id, Core.Domain.Entities.MachineSpecification machineSpecification)
        {
            // Fetch the MachineMaster to delete from the database
            var machinemasterToDelete = await _applicationDbContext.MachineSpecification.FirstOrDefaultAsync(u => u.Id == Id);

            // If the MachineMaster does not exist
            if (machinemasterToDelete is null)
            {
                return -1; //indicate failure
            }

            // Update the IsActive status to indicate deletion (or soft delete)
            machinemasterToDelete.IsDeleted = machineSpecification.IsDeleted;

            // Save changes to the database 
            await _applicationDbContext.SaveChangesAsync();

            return 1; // Indicate success
        }

        public async Task<bool> IsDuplicateSpecificationAsync(int machineId, int specificationId)
        {
            return await _applicationDbContext.MachineSpecification
                .AnyAsync(x => x.MachineId == machineId 
                            && x.SpecificationId == specificationId 
                            && x.IsDeleted == IsDelete.NotDeleted); // Only check active ones
        }
    }
}