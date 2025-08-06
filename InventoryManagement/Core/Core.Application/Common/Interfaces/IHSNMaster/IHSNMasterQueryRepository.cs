using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.HSNMaster.Queries.GetAllHSNMaster;
using Core.Application.HSNMaster.Queries.GetHSNMasterAutoComplete;

namespace Core.Application.Common.Interfaces.IHSNMaster
{
    public interface IHSNMasterQueryRepository
    {
        Task<(List<HSNMasterDto>, int)> GetAllAsync(int PageNumber, int PageSize, string SearchTerm);
        Task<HSNMasterDto> GetByIdAsync(int id);
        Task<List<GetHSNMasterAutoCompleteDto>> GetHSNMasterAutoCompleteAsync(string searchPattern);
        Task<bool> AlreadyExistsAsync(string hsnCode, int? id = null);
        Task<bool> NotFoundAsync(int id);
        Task<bool> FKColumnValidation(int id);        
        Task<bool> SoftDeleteValidation(int Id);     
        

               

    }
}