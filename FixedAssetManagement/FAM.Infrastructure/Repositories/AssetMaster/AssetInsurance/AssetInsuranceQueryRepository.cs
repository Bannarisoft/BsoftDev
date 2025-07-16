using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
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
    
    public async Task<(List<Core.Domain.Entities.AssetMaster.AssetInsurance>, int)> GetAllAssetInsuranceAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
                        var query = $$"""
                    DECLARE @TotalCount INT;
                    SELECT @TotalCount = COUNT(*) 
                    FROM FixedAsset.AssetInsurance 
                    WHERE IsDeleted = 0
                    {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (PolicyNo LIKE @Search OR VendorCode LIKE @Search)")}};

                    SELECT Id, AssetId, PolicyNo, StartDate, InsurancePeriod, EndDate, PolicyAmount, VendorCode, 
                        RenewalStatus, RenewedDate, CreatedBy, IsActive, IsDeleted
                    FROM FixedAsset.AssetInsurance
                    WHERE IsDeleted = 0
                    {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (PolicyNo LIKE @Search OR VendorCode LIKE @Search)")}}
                    ORDER BY Id DESC
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                    SELECT @TotalCount AS TotalCount;
                """;
                            var parameters = new
                    {
                        Search = $"%{SearchTerm}%",
                        Offset = (PageNumber - 1) * PageSize,
                        PageSize
                    };

                    var assetInsuranceData = await _dbConnection.QueryMultipleAsync(query, parameters);
                    var assetInsuranceList = (await assetInsuranceData.ReadAsync<Core.Domain.Entities.AssetMaster.AssetInsurance>()).ToList();
                    int totalCount = await assetInsuranceData.ReadFirstAsync<int>();

                    return (assetInsuranceList, totalCount);          
        }


        public async Task<Core.Domain.Entities.AssetMaster.AssetInsurance> GetByAssetIdAsync(int id)
            {
                const string query = @"
                    SELECT Id, AssetId,PolicyNo,StartDate, 
                        Insuranceperiod,  EndDate, PolicyAmount, VendorCode, RenewalStatus, 
                        RenewedDate, IsActive  FROM FixedAsset.AssetInsurance 
                    WHERE Id = @id AND IsDeleted = 0";

                return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AssetMaster.AssetInsurance>(query, new { id });
            }

          public async Task<bool> AlreadyExistsAsync(string PolicyNo, int? id = null)
        {   

              var query = "SELECT COUNT(1) FROM FixedAsset.AssetInsurance   WHERE PolicyNo = @PolicyNo AND IsDeleted = 0 ";
                var parameters = new DynamicParameters(new { PolicyNo = PolicyNo });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }
          public async Task<bool> ActiveInsuranceValidation(int AssetId, int? id = null)
        {   

              var query = "SELECT COUNT(1) FROM FixedAsset.AssetInsurance   WHERE AssetId = @AssetId AND IsDeleted = 0 AND IsActive = 1 ";
                var parameters = new DynamicParameters(new { AssetId = AssetId });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
        }

          


    }
}