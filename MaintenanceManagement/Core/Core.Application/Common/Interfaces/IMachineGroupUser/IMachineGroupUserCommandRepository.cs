using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IMachineGroupUser
{
    public interface IMachineGroupUserCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.MachineGroupUser  machineGroupUser);     
        Task<bool> UpdateAsync(Core.Domain.Entities.MachineGroupUser machineGroupUser);
        Task<bool> DeleteAsync(int id,Core.Domain.Entities.MachineGroupUser machineGroupUser); 
    }
}