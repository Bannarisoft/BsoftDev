using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IShiftMasterDetail
{
    public interface IShiftMasterDetailQuery
    {
         Task<(IEnumerable<dynamic>,int)> GetAllShiftMasterDetailAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<ShiftMasterDetail> GetByIdAsync(int ShiftMasterId);
        Task<List<ShiftMasterDetail>> GetShiftMasterDetail(string searchPattern);
        Task<bool> SoftDeleteValidation(int Id); 
        Task<bool> AlreadyExistsAsync(int ShiftMasterId);
        Task<bool> NotFoundAsync(int Id );
        Task<bool> FKColumnValidation(int ShiftMasterId );
    }
}