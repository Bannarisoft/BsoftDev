using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.CardView;
using FAM.Infrastructure.Data;
using Dapper;

namespace FAM.Infrastructure.Repositories.Dashboard
{
    public class DashboardQueryRepository : IDashboardQueryRepository
    {

         private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _iPAddressService;

        public DashboardQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection; 
            _iPAddressService = iPAddressService;
        }

        public async Task<AssetDashboardDto> GetDashboardDataAsync()
        {
           var UnitId = _iPAddressService.GetUnitId();
            var dashboard = new AssetDashboardDto();
              
                var cardViewQuery = @"
                    SELECT
                        COUNT(*) AS TotalAssets,
                        SUM(ISNULL(ap.PurchaseValue, 0)) AS TotalAssetValue,
                        SUM(CASE WHEN am.CreatedDate >= DATEADD(DAY, -30, GETDATE()) THEN 1 ELSE 0 END) AS NewAssets,
                        SUM(CASE WHEN am.CreatedDate >= DATEADD(DAY, -30, GETDATE()) THEN ISNULL(ap.PurchaseValue, 0) ELSE 0 END) AS NewAssetsValue,
                        (
                            SELECT COUNT(*)
                            FROM FixedAsset.AssetDisposal d
                            INNER JOIN FixedAsset.AssetMaster am2 ON d.AssetId = am2.Id
                            WHERE am2.IsDeleted = 0 AND am2.UnitId = @UnitId
                        ) AS AssetDisposed
                    FROM FixedAsset.AssetMaster am
                    LEFT JOIN FixedAsset.AssetPurchaseDetails ap ON am.Id = ap.AssetId
                    WHERE am.IsDeleted = 0 AND am.UnitId = @UnitId;
                ";

                var groupSummaryQuery = @"
                    SELECT 
                        ag.GroupName,
                        COUNT(am.Id) AS AssetCount,
                        SUM(ISNULL(ap.PurchaseValue, 0)) AS TotalPurchaseValue
                    FROM FixedAsset.AssetMaster am
                    INNER JOIN FixedAsset.AssetGroup ag ON am.AssetGroupId = ag.Id
                    LEFT JOIN FixedAsset.AssetPurchaseDetails ap ON am.Id = ap.AssetId
                    WHERE am.IsDeleted = 0 AND am.UnitId = @UnitId
                    GROUP BY ag.GroupName
                    ORDER BY ag.GroupName;
                ";

              // Execute using Dapper
            dashboard.CardView = await _dbConnection.QueryFirstOrDefaultAsync<CardViewDto>(cardViewQuery, new { UnitId  });
            var groupData = await _dbConnection.QueryAsync<AssetGroupSummaryDto>(groupSummaryQuery, new { UnitId });
            dashboard.GroupSummary = groupData.AsList();

            return dashboard;
        
    
            
        }
    }
}