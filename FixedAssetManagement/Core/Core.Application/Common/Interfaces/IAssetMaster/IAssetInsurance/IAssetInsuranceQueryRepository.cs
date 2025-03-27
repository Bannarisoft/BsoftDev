using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance
{
    public interface IAssetInsuranceQueryRepository 
    {
            Task<Core.Domain.Entities.AssetMaster.AssetInsurance>  GetByAssetIdAsync(int id );            

            Task<(List<Core.Domain.Entities.AssetMaster.AssetInsurance>,int)> GetAllAssetInsuranceAsync(int PageNumber, int PageSize, string? SearchTerm);
            Task<bool> AlreadyExistsAsync(string PolicyNo, int? id = null);
             Task<bool> ActiveInsuranceValidation(int AssetId, int? id = null);
           
           
        
    }
}