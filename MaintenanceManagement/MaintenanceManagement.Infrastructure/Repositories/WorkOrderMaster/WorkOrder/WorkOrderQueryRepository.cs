using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Dapper;
using MassTransit;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrderMaster.WorkOrder
{
    public class WorkOrderQueryRepository : IWorkOrderQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public WorkOrderQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public Task<WorkOrderDto> GetByIdAsync(int workOrderId)
        {
            throw new NotImplementedException();
        }

        /* public async Task<WorkOrderDto> GetByIdAsync(int workOrderId)
       {
            const string query = @"            
           SELECT AM.Id, AM.CompanyId, AM.UnitId, AM.AssetCode, AM.AssetName, AM.AssetGroupId, AM.AssetCategoryId, AM.AssetSubCategoryId, AM.AssetParentId, 
               AM.AssetType, AM.MachineCode, AM.Quantity, AM.UOMId, AM.AssetDescription, AM.WorkingStatus, AM.AssetImage, AM.ISDepreciated, AM.IsTangible, 
               AM.IsActive, AM.CreatedBy, AM.CreatedDate, AM.CreatedByName, AM.CreatedIP, AM.ModifiedBy, AM.ModifiedDate, AM.ModifiedByName, AM.ModifiedIP,
               AG.GroupName AS AssetGroupName, AC.CategoryName AS AssetCategoryDesc, A.Description AS AssetSubCategoryDesc, U.UOMName, 
               MM.Description AS WorkingStatusDesc, M.Description AS AssetTypeDesc, ISNULL(AM1.AssetDescription, '') AS ParentAssetDesc,
               (SELECT A.Id AS SpecificationId, A.SpecificationValue, SM.SpecificationName 
                   FROM FixedAsset.AssetSpecifications AS A
                   INNER JOIN FixedAsset.SpecificationMaster SM ON SM.Id = A.SpecificationId    
                   WHERE A.AssetId = AM.Id AND A.IsDeleted = 0 
                   FOR JSON PATH) AS SpecificationsJson   
           FROM FixedAsset.AssetMaster AM
           INNER JOIN FixedAsset.AssetGroup AG ON AG.Id = AM.AssetGroupId
           INNER JOIN FixedAsset.AssetCategories AC ON AC.Id = AM.AssetCategoryId
           INNER JOIN FixedAsset.AssetSubCategories A ON A.Id = AM.AssetSubCategoryId
           INNER JOIN FixedAsset.UOM U ON U.Id = AM.UOMId
           INNER JOIN FixedAsset.MiscMaster MM ON MM.Id = AM.WorkingStatus
           LEFT JOIN FixedAsset.MiscMaster M ON M.Id = AM.AssetType
           LEFT JOIN FixedAsset.AssetMaster AM1 ON AM1.Id = AM.AssetParentId
           WHERE AM.Id = @assetId AND AM.IsDeleted = 0";

          var assetMaster = await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGeneralDTO>(query, new { assetId });

           if (assetMaster is null)
           {
               throw new KeyNotFoundException($"DepreciationGroup with ID {assetId} not found.");
           }

           // 🔹 Deserialize JSON directly for the single object
           if (!string.IsNullOrEmpty(assetMaster.SpecificationsJson))
           {
               assetMaster.Specifications = JsonConvert.DeserializeObject<List<AssetSpecificationDTO>>(assetMaster.SpecificationsJson) ?? new();
           }
           else
           {
               assetMaster.Specifications = new List<AssetSpecificationDTO>();
           }
           return assetMaster; 
       }*/

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOPriorityDescAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOPriority.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWORequestTypeDescAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder           
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WORequestType.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<(dynamic WorkOrderResult, IEnumerable<dynamic> Activity, IEnumerable<dynamic> Schedule, IEnumerable<dynamic> Item, IEnumerable<dynamic> Technician)> GetWorkOrderByIdAsync(int workOrderId)
        {
            var sqlQuery = @"
                -- First Query: AssetMaster (One-to-One)

            SELECT  WorkOrderTypeId,MC.CategoryName WorkOrderTypeDesc,	RequestId,MM1.Id PriorityId,M1.Code PriorityDesc,Remarks,
            MM2.Description+'\'+C.CompanyName+'\'+UN.ShortName +'\'+WO.Image WOImage,MM.Id StatusId,MM1.Code StatusDesc,VendorId,RootCauseId,MM3.Code RootCauseDesc
            FROM Maintenance.WorkOrder WO
            INNER JOIN Maintenance.MaintenanceCategory MC on MC.Id=WO.WorkOrderTypeId
            INNER JOIN Maintenance.MiscTypeMaster MM ON MM.MiscTypeCode='Status'
            INNER JOIN Maintenance.MiscMaster  M ON M.MiscTypeId=MM.Id
            INNER JOIN Maintenance.MiscTypeMaster MM1 ON MM1.MiscTypeCode='Priority'
            INNER JOIN Maintenance.MiscMaster  M1 ON M1.MiscTypeId=MM1.Id
            INNER JOIN Maintenance.MiscTypeMaster MM2 on MM2.MiscTypeCode ='WOImage'
            INNER JOIN Maintenance.MiscTypeMaster MM3 ON MM3.MiscTypeCode='RootCause'
            INNER JOIN Maintenance.MiscMaster  M2 ON M2.MiscTypeId=MM3.Id
            LEFT JOIN Bannari.AppData.Unit UN on UN.Id=WO.UnitId
            LEFT JOIN Bannari.AppData.Company C on C.Id=WO.CompanyId
            WHERE AM.Id = @workOrderId;

                SELECT AM.AssetName, AM.AssetCode, AM.Quantity, U.UOMName, AG.GroupName,AC.CategoryName, ASUBC.SubCategoryName, AssetParent.AssetName,AM.AssetGroupId ,
                MM.Description+'\'+C.CompanyName+'\'+UN.ShortName +'\'+AM.AssetImage AssetImage,AM.AssetCategoryId,AM.AssetSubCategoryId,
                AM.AssetParentId,AM.AssetType,AM.UOMId,AM.WorkingStatus
                FROM Maintenance.WorkOrder AM
                INNER JOIN Maintenance.[UOM] U ON U.Id = AM.UOMId
                INNER JOIN Maintenance.[AssetGroup] AG ON AM.AssetGroupId = AG.Id
                INNER JOIN Maintenance.[AssetCategories] AC ON AM.AssetCategoryId = AC.Id
                INNER JOIN Maintenance.[AssetSubCategories] ASUBC ON AM.AssetSubCategoryId = ASUBC.Id
                LEFT JOIN Maintenance.[AssetMaster] AssetParent ON AM.AssetParentId = AssetParent.Id
                LEFT JOIN FixedAsset.MiscTypeMaster MM on MM.MiscTypeCode ='GETASSETIMAGE'
                LEFT JOIN Bannari.AppData.Unit UN on UN.Id=AM.UnitId
                LEFT JOIN Bannari.AppData.Company C on C.Id=AM.CompanyId
                WHERE AM.Id = @AssetId;

                -- Second Query: AssetLocation (One-to-One)
                SELECT U.UnitName,D.DeptName,L.LocationName,SL.SubLocationName,U.OldUnitId,AL.CustodianId,AL.UserId,AL.DepartmentId,AL.LocationId,AL.SubLocationId
                FROM Maintenance.[AssetLocation] AL
                INNER JOIN Maintenance.[Location] L ON L.Id=AL.LocationId
                INNER JOIN Maintenance.[SubLocation] SL ON SL.Id=AL.SubLocationId
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AL.UnitId = U.Id
                LEFT JOIN [Bannari].[AppData].[Department] D ON AL.DepartmentId=D.Id                
                WHERE AL.AssetId = @AssetId;
                

                -- Third Query: AssetPurchaseDetails (One-to-Many)
                SELECT AP.Id,AP.VendorCode, AP.VendorName,U.UnitName,ASource.SourceName,AP.GrnNo,Cast(AP.GrnDate AS date) AS GrnDate ,
                AP.GrnSno,AP.GrnValue,AP.PoNo,Cast(AP.PoDate AS date) AS PoDate,AP.PurchaseValue,AP.AcceptedQty,AP.Uom,
                AP.PoSno,AP.ItemCode,AP.ItemName,AP.BillNo,Cast(AP.BillDate AS date) AS BillDate ,AP.BinLocation 
                ,AP.PjYear,AP.PjDocId,AP.PjDocSr,AP.PjDocNo,AP.AssetSourceId ,cast(AP.CapitalizationDate as date)CapitalizationDate
                FROM Maintenance.[AssetPurchaseDetails] AP
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AP.OldUnitId = U.OldUnitId
                INNER JOIN Maintenance.[AssetSource] ASource ON ASource.Id=AP.AssetSourceId
                WHERE AP.AssetId = @AssetId;

                SELECT A.Id,SM.SpecificationName,A.SpecificationValue,A.SpecificationId,SM.IsDefault FROM  Maintenance.[AssetSpecifications] A
                INNER JOIN Maintenance.[SpecificationMaster] SM ON SM.Id=A.SpecificationId
                WHERE A.AssetId=@AssetId

                SELECT Aw.Id,CAST(AW.StartDate AS DATE) AS StartDate,CAST(AW.EndDate AS DATE) AS EndDate,AW.Period,MMWaranty.description AS WarrantyType,MMClaim.description AS ServiceClaimStatus,
                AW.WarrantyProvider,AW.MobileNumber,AW.ContactPerson,AW.Description,AW.Email,AW.Document,C.CountryName,S.StateName,City.CityName,
                AW.ServiceAddressLine1,AW.ServiceAddressLine2,
                AW.ServicePinCode,AW.ServiceContactPerson,AW.ServiceMobileNumber,AW.ServiceEmail,AW.ServiceClaimProcessDescription,
                CAST(AW.ServiceLastClaimDate AS DATE) AS ServiceLastClaimDate,AW.WarrantyType AS WarrantyTypeId,
                AW.ServiceClaimStatus AS ServiceClaimStatusId,AW.ServiceCountryId,AW.ServiceStateId,AW.ServiceCityId 
                FROM Maintenance.[AssetWarranty] AW
                INNER JOIN Maintenance.[MiscMaster] MMWaranty ON MMWaranty.Id=AW.WarrantyType
                INNER JOIN Maintenance.[MiscMaster] MMClaim ON MMClaim.Id=AW.ServiceClaimStatus
                INNER JOIN [Bannari].[AppData].[Country] C ON C.Id=AW.ServiceCountryId
                INNER JOIN [Bannari].[AppData].[State] S ON S.Id=AW.ServiceStateId
                INNER JOIN [Bannari].[AppData].[City] City ON City.Id=AW.ServiceCityId
                WHERE AW.AssetId=@AssetId

                SELECT AA.Id,CAST(AA.StartDate AS DATE) AS StartDate,CAST(AA.EndDate AS DATE) AS EndDate,AA.Period,AA.VendorCode,AA.VendorName,
                MMCoverage.description AS CoverageType,
                MMRenewal.description AS RenewalStatus,CAST(AA.RenewedDate AS DATE) AS RenewedDate,AA.CoverageType AS CoverageTypeId,
                AA.RenewalStatus AS RenewalStatusId,AA.IsActive,AA.FreeServiceCount,AA.VendorEmail,AA.VendorPhone
                FROM Maintenance.[AssetAmc] AA
                INNER JOIN Maintenance.[MiscMaster] MMCoverage ON MMCoverage.Id=AA.CoverageType
                INNER JOIN Maintenance.[MiscMaster] MMRenewal ON MMRenewal.Id=AA.RenewalStatus
                WHERE AA.AssetId=@AssetId

                SELECT AD.Id,MMDisposal.description AS DisposalType,CAST(AD.DisposalDate AS DATE) AS DisposalDate,AD.DisposalReason,
                AD.DisposalAmount,AD.DisposalType AS DisposalTypeId  ,AD.AssetPurchaseId
                FROM Maintenance.[AssetDisposal] AD
                INNER JOIN Maintenance.[MiscMaster] MMDisposal ON MMDisposal.Id=AD.DisposalType
                WHERE AD.AssetId=@AssetId

                SELECT Id, PolicyNo,CAST(StartDate AS DATE) AS StartDate,CAST(EndDate AS DATE) AS EndDate,Insuranceperiod,PolicyAmount,
                VendorCode,RenewalStatus,CAST(RenewedDate AS DATE) AS RenewedDate,IsActive
                FROM Maintenance.[AssetInsurance]
                WHERE AssetId=@AssetId
                ";

            using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { AssetId = workOrderId });

            var WorkOrderResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var Activity = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var Schedule = await multi.ReadAsync<dynamic>();
            var Item = await multi.ReadAsync<dynamic>();
            var Technician = await multi.ReadAsync<dynamic>();            
           
            return (WorkOrderResult, Activity, Schedule, Item, Technician);
       
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync()
        {
           const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY SortOrder DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        
    } 
}
   