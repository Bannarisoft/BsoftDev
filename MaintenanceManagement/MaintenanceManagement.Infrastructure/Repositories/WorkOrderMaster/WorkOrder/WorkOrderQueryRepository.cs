using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Common;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.WorkOrderMaster.WorkOrder
{
    public class WorkOrderQueryRepository : IWorkOrderQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public WorkOrderQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<(dynamic WorkOrderResult, IEnumerable<dynamic> Activity, IEnumerable<dynamic> Schedule, IEnumerable<dynamic> Item, IEnumerable<dynamic> Technician)> GetAllWorkOrderAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOPriorityDescAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOPriority.MiscCode };        
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

            using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { AssetId = assetId });

            var assetResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var locationResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var purchaseDetails = await multi.ReadAsync<dynamic>();
            var SpecDetails = await multi.ReadAsync<dynamic>();
            var WarrantyDetails = await multi.ReadAsync<dynamic>();
            var AMCDetails = await multi.ReadAsync<dynamic>();
            var DisposalResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            var InsuranceDetails = await multi.ReadAsync<dynamic>();

       
            if (locationResult != null && !string.IsNullOrEmpty(locationResult.OldUnitId))
            {
                // Fetch Custodian
                if (locationResult.CustodianId > 0)
                {
                    var custodianParams = new
                    {
                        DivCode = locationResult.OldUnitId,
                        EmpNo = locationResult.CustodianId
                    };

                    var custodianEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                        "dbo.GetEmployeeByDivision",
                        custodianParams,
                        commandType: CommandType.StoredProcedure
                    );

                    if (custodianEmployee != null)
                        locationResult.CustodianName = custodianEmployee.Empname;
                }

                // Fetch User
                if (locationResult.UserId > 0)
                {
                    var userParams = new
                    {
                        DivCode = locationResult.OldUnitId,
                        EmpNo = locationResult.UserId
                    };

                    var userEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                        "dbo.GetEmployeeByDivision",
                        userParams,
                        commandType: CommandType.StoredProcedure
                    );

                    if (userEmployee != null)
                        locationResult.UserName = userEmployee.Empname;
                }
            }

            return (assetResult, locationResult, purchaseDetails, SpecDetails, WarrantyDetails, AMCDetails, DisposalResult, InsuranceDetails);
       
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWOStatusDescAsync()
        {
           const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        
    }
}
   