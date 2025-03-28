using System.Data;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral
{
    public class AssetDetailsQueryRepository : IAssetDetailsQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public AssetDetailsQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
         public async Task<(dynamic AssetResult, dynamic LocationResult, IEnumerable<dynamic> PurchaseDetails, IEnumerable<dynamic> Spec, IEnumerable<dynamic> Warranty,IEnumerable<dynamic> Amc,dynamic Disposal, IEnumerable<dynamic> Insurance )> GetAssetMasterByIdAsync(int assetId)
        {
             var sqlQuery = @"
                  -- First Query: AssetMaster (One-to-One)
                  SELECT AM.AssetName, AM.AssetCode, AM.Quantity, U.UOMName, AG.GroupName, 
                         AC.CategoryName, ASUBC.SubCategoryName, AssetParent.AssetName 
                  FROM [FixedAsset].[AssetMaster] AM
                  INNER JOIN [FixedAsset].[UOM] U ON U.Id = AM.UOMId
                  INNER JOIN [FixedAsset].[AssetGroup] AG ON AM.AssetGroupId = AG.Id
                  INNER JOIN [FixedAsset].[AssetCategories] AC ON AM.AssetCategoryId = AC.Id
                  INNER JOIN [FixedAsset].[AssetSubCategories] ASUBC ON AM.AssetSubCategoryId = ASUBC.Id
                  LEFT JOIN [FixedAsset].[AssetMaster] AssetParent ON AM.AssetParentId = AssetParent.Id
                  WHERE AM.Id = @AssetId;

                  -- Second Query: AssetLocation (One-to-One)
                  SELECT U.UnitName,D.DeptName,L.LocationName,SL.SubLocationName FROM [FixedAsset].[AssetLocation] AL
                    INNER JOIN [FixedAsset].[Location] L ON L.Id=AL.LocationId
                    INNER JOIN [FixedAsset].[SubLocation] SL ON SL.Id=AL.SubLocationId
                    LEFT JOIN [Bannari].[AppData].[Unit] U ON AL.UnitId = U.Id
                    LEFT JOIN [Bannari].[AppData].[Department] D ON AL.DepartmentId=D.Id
                  WHERE AL.AssetId = @AssetId;

                  -- Third Query: AssetPurchaseDetails (One-to-Many)
                  SELECT AP.VendorCode, AP.VendorName,U.UnitName,ASource.SourceName,AP.GrnNo,Cast(AP.GrnDate AS date) AS GrnDate ,
                  AP.GrnSno,AP.GrnValue,AP.PoNo,Cast(AP.PoDate AS date) AS PoDate,AP.PurchaseValue,AP.AcceptedQty,AP.Uom,
                  AP.PoSno,AP.ItemCode,AP.ItemName,AP.BillNo,Cast(AP.BillDate AS date) AS BillDate ,AP.BinLocation 
                  FROM [FixedAsset].[AssetPurchaseDetails] AP
                  LEFT JOIN [Bannari].[AppData].[Unit] U ON AP.OldUnitId = U.OldUnitId
				  INNER JOIN [FixedAsset].[AssetSource] ASource ON ASource.Id=AP.AssetSourceId
                  WHERE AP.AssetId = @AssetId;

                SELECT SM.SpecificationName,A.SpecificationValue,A.SpecificationId FROM  [FixedAsset].[AssetSpecifications] A
				INNER JOIN [FixedAsset].[SpecificationMaster] SM ON SM.Id=A.SpecificationId
				WHERE A.AssetId=@AssetId

                SELECT CAST(AW.StartDate AS DATE) AS StartDate,CAST(AW.EndDate AS DATE) AS EndDate,AW.Period,MMWaranty.description AS WarrantyType,MMClaim.description AS ServiceClaimStatus,
				AW.WarrantyProvider,AW.MobileNumber,AW.ContactPerson,AW.Description,AW.Email,AW.Document,C.CountryName,S.StateName,City.CityName,
                AW.ServiceAddressLine1,AW.ServiceAddressLine2,
				AW.ServicePinCode,AW.ServiceContactPerson,AW.ServiceMobileNumber,AW.ServiceEmail,AW.ServiceClaimProcessDescription,
                CAST(AW.ServiceLastClaimDate AS DATE) AS ServiceLastClaimDate,AW.WarrantyType AS WarrantyTypeId,
                AW.ServiceClaimStatus AS ServiceClaimStatusId,AW.ServiceCountryId,AW.ServiceStateId,AW.ServiceCityId 
				FROM [FixedAsset].[AssetWarranty] AW
				INNER JOIN [FixedAsset].[MiscMaster] MMWaranty ON MMWaranty.Id=AW.WarrantyType
				INNER JOIN [FixedAsset].[MiscMaster] MMClaim ON MMClaim.Id=AW.ServiceClaimStatus
				INNER JOIN [Bannari].[AppData].[Country] C ON C.Id=AW.ServiceCountryId
				INNER JOIN [Bannari].[AppData].[State] S ON S.Id=AW.ServiceStateId
				INNER JOIN [Bannari].[AppData].[City] City ON City.Id=AW.ServiceCityId
				WHERE AW.AssetId=@AssetId

                SELECT CAST(AA.StartDate AS DATE) AS StartDate,CAST(AA.EndDate AS DATE) AS EndDate,AA.Period,AA.VendorCode,AA.VendorName,
                MMCoverage.description AS CoverageType,
				MMRenewal.description AS RenewalStatus,CAST(AA.RenewedDate AS DATE) AS RenewedDate,AA.CoverageType AS CoverageTypeId,
                AA.RenewalStatus AS RenewalStatusId,AA.IsActive
				FROM [FixedAsset].[AssetAmc] AA
				INNER JOIN [FixedAsset].[MiscMaster] MMCoverage ON MMCoverage.Id=AA.CoverageType
				INNER JOIN [FixedAsset].[MiscMaster] MMRenewal ON MMRenewal.Id=AA.RenewalStatus
				WHERE AA.AssetId=@AssetId

                SELECT MMDisposal.description AS DisposalType,CAST(AD.DisposalDate AS DATE) AS DisposalDate,AD.DisposalReason,
                AD.DisposalAmount,AD.DisposalType AS DisposalTypeId  
                FROM [FixedAsset].[AssetDisposal] AD
				INNER JOIN [FixedAsset].[MiscMaster] MMDisposal ON MMDisposal.Id=AD.DisposalType
				WHERE AD.AssetId=@AssetId

                SELECT PolicyNo,CAST(StartDate AS DATE) AS StartDate,CAST(EndDate AS DATE) AS EndDate,Insuranceperiod,PolicyAmount,
                VendorCode,RenewalStatus,CAST(RenewedDate AS DATE) AS RenewedDate,IsActive
                FROM [FixedAsset].[AssetInsurance]
				WHERE AssetId=@AssetId
              ";

              using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { AssetId = assetId });

              var assetResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
              var locationResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
              var purchaseDetails = await multi.ReadAsync<dynamic>();
              var SpecDetails = await multi.ReadAsync<dynamic>();
              var WarrantyDetails = await multi.ReadAsync<dynamic>();
              var AMCDetails = await multi.ReadAsync<dynamic>();
              var DisposalResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
              var InsuranceDetails = await multi.ReadAsync<dynamic>();

              return (assetResult, locationResult, purchaseDetails, SpecDetails, WarrantyDetails, AMCDetails, DisposalResult, InsuranceDetails);
       
        }

        public async Task<AssetLocationDto> GetAssetLocationByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetLocationDto>(
                "SELECT * FROM FixedAsset.AssetLocation WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }

        public async Task<AssetPurchaseDetailsDto> GetAssetPurchaseByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetPurchaseDetailsDto>(
                "SELECT * FROM FixedAsset.AssetPurchaseDetails WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }

        public async Task<AssetAmcDto> GetAssetAMCByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetAmcDto>(
                "SELECT * FROM FixedAsset.AssetAmc WHERE AssetId = @AssetId and Isdeleted=0" , new { AssetId = assetId });
        }

        public async Task<AssetWarrantyDTO> GetAssetWarrantyByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetWarrantyDTO>(
                "SELECT * FROM FixedAsset.AssetWarranty WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }

        public async Task<AssetSpecificationDTO> GetAssetSpecificationByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetSpecificationDTO>(
                "SELECT * FROM FixedAsset.AssetSpecifications WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }

        public async Task<AssetDisposalDto> GetAssetDisposalByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetDisposalDto>(
                "SELECT * FROM FixedAsset.AssetDisposal WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }
    public async Task<GetAssetInsuranceDto> GetAssetInsuranceByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<GetAssetInsuranceDto>(
                "SELECT * FROM FixedAsset.AssetDisposal WHERE AssetId = @AssetId and Isdeleted=0", new { AssetId = assetId });
        }
            public async Task<AssetAdditionalCostDto> GetAssetAdditionalCostByIdAsync(int assetId)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<AssetAdditionalCostDto>(
                "SELECT * FROM FixedAsset.AssetAdditionalCost WHERE AssetId = @AssetId ", new { AssetId = assetId });
        }
    }    
}