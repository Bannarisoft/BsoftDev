using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetPurchase
{
    public class AssetPurchaseQueryRepository : IAssetPurchaseQueryRepository
    {
         private readonly IDbConnection _dbConnection; 

        public AssetPurchaseQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<AssetGrnItem>> GetAssetGrnItem(int OldUnitId, int GrnNo)
        {
            const string query = @"
            SELECT grnslno as GrnSerialNo,idesc as ItemName
            FROM dbo.GetGRNSNO(@OldUnitId,@GrnNo,NULL) order by grnslno asc";

            var parameters = new { OldUnitId, GrnNo };
            var grnList = await _dbConnection.QueryAsync<AssetGrnItem>(query, parameters);

            return grnList?.ToList() ?? new List<AssetGrnItem>();
        }

        public async Task<List<AssetGrnDetails>> GetAssetGrnItemDetails(int OldUnitId, int GrnNo, int GrnSerialNo)
        {
             const string query = @"
            SELECT
            budtype as BudgetType, 
            unitcode as OldUnitId,
            slcode as VendorCode,
            slname as VendorName,
            podt as PoDate,
            pono as PoNo,
            poslno as PoSno,
            itemcode as ItemCode,
            idesc as ItemName,
            grnno as GrnNo,
            grnslno as GrnSno,
            grndt as GrnDate,
            qcflg as QcCompleted,
            acpqty as AcceptedQty,
            itmval as PurchaseValue,
            grnval as GrnValue,
            billno as BillNo,
            billdt as BillDate,
            iuom as Uom,
            binloc as BinLocation,
            acyr as PjYear,
            docid as PjDocId,
            docsr as PjDocSr,
            docno as PjDocNo
            FROM 
            dbo.GetGRNSNO(@OldUnitId,@GrnNo,@GrnSerialNo)";

            var parameters = new { OldUnitId, GrnNo, GrnSerialNo };
            var grnList = await _dbConnection.QueryAsync<AssetGrnDetails>(query, parameters);

            return grnList?.ToList() ?? new List<AssetGrnDetails>();
        }

        public async Task<List<AssetGrn>> GetAssetGrnNo(int OldUnitId)
        {
             const string query = @"
            SELECT DISTINCT grnno AS GrnNo, unitcode AS OldUnitId
            FROM dbo.GetGRNDetails(@OldUnitId)";

            var parameters = new { OldUnitId };
            var grnList = await _dbConnection.QueryAsync<AssetGrn>(query, parameters);

            return grnList?.ToList() ?? new List<AssetGrn>();
        }

        public async Task<List<AssetSource>> GetAssetSources(string searchPattern)
        {
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues

            const string query = @"
             SELECT Id, SourceName 
            FROM FixedAsset.AssetSource 
            WHERE IsDeleted = 0 
            AND SourceName LIKE @SearchPattern";  
            var parameters = new 
            { 
            SearchPattern = $"%{searchPattern}%" 
            };

            var assetSources = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetSource>(query, parameters);
            return assetSources.ToList();
        }

        public async Task<List<AssetUnit>> GetAssetUnit(string userName)
        {
         

            const string query = @"
            SELECT C.OldUnitId, C.UnitName 
            FROM Bannari.AppSecurity.Users A
            INNER JOIN Bannari.AppSecurity.UserUnit B ON A.UserId = B.UserId
            INNER JOIN Bannari.AppData.Unit C ON B.UnitId = C.Id
            WHERE B.IsActive = 1 
            AND A.IsActive = 1 
            AND A.UserName = @UserName
             ORDER BY C.UnitName ASC";
    
             var parameters = new { UserName = userName };

            var assetSources = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetPurchase.AssetUnit>(query, parameters);
            return assetSources.ToList();
        }
    }
}