using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.MRS.Queries;
using Core.Application.MRS.Queries.GetCategory;
using Core.Application.MRS.Queries.GetDepartment;
using Core.Application.MRS.Queries.GetPendingQty;
using Core.Application.MRS.Queries.GetSubCostCenter;
using Core.Application.MRS.Queries.GetSubDepartment;

namespace Core.Application.Common.Interfaces.IMRS
{
    public interface IMRSQueryRepository
    {
        Task<List<MDepartmentDto>> GetMDepartment(string OldUnitcode);
        Task<List<MSubCostCenterDto>> GetSubCostCenter(string OldUnitcode);
        Task<List<MCategoryDto>> GetCategory(string OldUnitcode);
        Task<List<MSubDepartment>> GetSubDepartment(string OldUnitcode);
        Task<GetPendingQtyDto?> GetPendingIssueAsync(string oldUnitCode, string itemCode);
       
       
    }
}