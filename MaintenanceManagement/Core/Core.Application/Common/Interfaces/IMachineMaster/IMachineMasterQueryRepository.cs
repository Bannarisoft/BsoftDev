using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineNoDepartmentbyId;

namespace Core.Application.Common.Interfaces.IMachineMaster
{
    public interface IMachineMasterQueryRepository
    {
        Task<MachineMasterDto?> GetByIdAsync(int Id);
        Task<List<MachineMasterDto>> GetAllMachineAsync(string? SearchTerm);
        Task<List<Core.Domain.Entities.MachineMaster>> GetMachineAsync(string searchPattern);
        Task<List<GetMachineNoDepartmentbyIdDto>> GetMachineNoDepartmentAsync(int DepartmentId);
        Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupAsync(int MachineGroupId);
        // Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroup(int MachineGroupId);
        Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupSagaAsync(int MachineGroupId, int UnitId);
        Task<List<Core.Domain.Entities.MiscMaster>> GetMachineLineNoAsync();
        Task<MachineDepartmentGroupDto?> GetMachineDepartment(int MachineGroupId);
        Task<bool> NotFoundAsync(int id);
    }
}