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
using Core.Application.Dashboard.AssetExpired;
using Core.Application.Dashboard.Common;

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

         public async Task<ChartDto> GetAssetExpiredDashBoardDataAsync()
        {
            var unitId = _iPAddressService.GetUnitId(); // Or however you're getting UnitId

            var query = @"
                DECLARE @StartDate DATE = 
                    DATEFROMPARTS(
                        CASE 
                            WHEN MONTH(GETDATE()) >= 4 THEN YEAR(GETDATE()) 
                            ELSE YEAR(GETDATE()) - 1 
                        END, 4, 1
                    );

                DECLARE @EndDate DATE = 
                    DATEFROMPARTS(
                        CASE 
                            WHEN MONTH(GETDATE()) >= 4 THEN YEAR(GETDATE()) + 1 
                            ELSE YEAR(GETDATE()) 
                        END, 3, 31
                    );

                SELECT 
                    G.GroupName,
                    COUNT(*) AS ExpiredAssetCount,
                    SUM(B.PurchaseValue) * (ISNULL(D.ResidualValue, 0) / 100.0) AS ResidualValueAmount
                FROM 
                    FixedAsset.AssetMaster A
                    INNER JOIN FixedAsset.AssetPurchaseDetails B ON A.Id = B.AssetId
                    INNER JOIN FixedAsset.AssetGroup G ON A.AssetGroupId = G.Id
                    LEFT JOIN FixedAsset.DepreciationGroups D ON D.AssetGroupId = G.Id
                WHERE 
                    A.UnitId = @UnitId
                    AND DATEADD(YEAR, D.UsefulLife, B.PoDate) BETWEEN @StartDate AND @EndDate
                GROUP BY 
                    G.GroupName, D.ResidualValue
                ORDER BY 
                    G.GroupName ASC;
            ";

            var result = await _dbConnection.QueryAsync<AssetExpiredDashBoardDto>(query, new { UnitId = unitId });

            return new ChartDto
            {
                Categories = result.Select(x => x.GroupName).ToList(),
                Series = new List<ChartSeriesDto>
                {
                    new ChartSeriesDto
                    {
                        Name = "Expired Assets",
                        Data = result.Select(x => (decimal)x.ExpiredAssetCount).ToList()
                    },
                    new ChartSeriesDto
                    {
                        Name = "Residual Value",
                        Data = result.Select(x => x.ResidualValueAmount).ToList()
                    }
                }
            };
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