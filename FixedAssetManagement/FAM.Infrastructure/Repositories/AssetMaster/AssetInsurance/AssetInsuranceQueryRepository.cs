using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetInsurance
{
    public class AssetInsuranceQueryRepository : IAssetInsuranceQueryRepository
    {
       private readonly IDbConnection _dbConnection;
          public AssetInsuranceQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        

        public async Task<Core.Domain.Entities.AssetMaster.AssetInsurance> GetByAssetIdAsync(int id)
            {
                const string query = @"
                    SELECT Id, AssetId,PolicyNo,StartDate, 
                        Insuranceperiod,  EndDate, PolicyAmount, VendorCode, RenewalStatus, 
                        RenewedDate, InsuranceStatus  FROM FixedAsset.AssetInsurance 
                    WHERE Id = @id";

                return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AssetMaster.AssetInsurance>(query, new { id });
            }

    }
}