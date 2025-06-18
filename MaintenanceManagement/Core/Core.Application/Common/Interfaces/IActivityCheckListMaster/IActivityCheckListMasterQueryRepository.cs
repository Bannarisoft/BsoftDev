using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityCheckListMaster.Queries.GetActivityCheckListMaster;
using Core.Application.ActivityCheckListMaster.Queries.GetCheckListByActivityId;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IActivityCheckListMaster
{
    public interface IActivityCheckListMasterQueryRepository
    {
        Task<(List<GetAllActivityCheckListMasterDto>, int)> GetAllActivityCheckListMasterAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<GetAllActivityCheckListMasterDto> GetByIdAsync(int Id);

        Task<bool> GetByActivityCheckListAsync(string activityChecklist, int activityId);

        Task<bool> AlreadyExistsCheckListAsync(string activityChecklist, int activityId, int? id = null);

        // Task<List<GetActivityCheckListByActivityIdDto>> GetCheckListByActivityIdAsync( int  Id) ;
        //    Task<List<GetActivityCheckListByActivityIdDto>> GetCheckListByActivityIdsAsync(List<int> ids);

        Task<List<GetActivityCheckListByActivityIdDto>> GetCheckListByActivityIdsAsync(List<int> ids, int? workOrderId = null);
        
    //    Task<bool> SoftDeleteValidation(int Id); 


                                                              

             

    }
}