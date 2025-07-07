using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.IMachineSpecification
{
    public interface IMachineSpecificationCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.MachineSpecification machineSpecification);
        Task<int> DeleteAsync(int Id, Core.Domain.Entities.MachineSpecification machineSpecification);
        Task<bool> IsDuplicateSpecificationAsync(int machineId, int specificationId);
    }
}