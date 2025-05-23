using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Queries.GetActivityByMachinGroupId;
using Core.Application.ActivityMaster.Queries.GetAllActivityMaster;
using Core.Application.ActivityMaster.Queries.GetMachineGroupById;
using Core.Application.MachineGroup.Queries.GetMachineGroupById;

namespace Core.Application.Common.Interfaces.IActivityMaster
{
    public interface IActivityMasterQueryRepository
    {

        //Task<(List<Core.Domain.Entities.ActivityMaster>,int)> GetAllActivityMasterAsync(int PageNumber, int PageSize, string? SearchTerm);

        //  Task<(List<GetAllActivityMasterDto>, int)> GetAllActivityMasterAsync(int PageNumber, int PageSize ,string? SearchTerm );

        Task<(List<GetAllActivityMasterDto>, int)> GetAllActivityMasterAsync(int PageNumber, int PageSize, string? SearchTerm);

        //  Task<Core.Domain.Entities.ActivityMaster> GetByIdAsync(int id);

        Task<GetActivityMasterByIdDto> GetByIdAsync(int id);

        Task<List<GetMachineGroupNameByIdDto>> GetMachineGroupById(int activityId);

        Task<List<Core.Domain.Entities.ActivityMaster>> GetActivityMasterAutoComplete(string searchPattern);


        Task<bool> GetByActivityNameAsync(string activityname);
        Task<bool> FKColumnExistValidation(int activityId);

        Task<List<Core.Domain.Entities.MiscMaster>> GetActivityTypeAsync();
            
        Task<List<GetActivityByMachinGroupDto>> GetActivityByMachinGroupId(int MachineGroupId);    
        
    }
}