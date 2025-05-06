using System.Data;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using Dapper;
using FAM.Infrastructure.Repositories.Common;
using Newtonsoft.Json;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral
{
    public class AssetMasterGeneralQueryRepository : BaseQueryRepository,IAssetMasterGeneralQueryRepository
    {
        private readonly IDbConnection _dbConnection;        
        public AssetMasterGeneralQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
            : base(ipAddressService) 
        {
            _dbConnection = dbConnection;            
        }     
        public async Task<(List<AssetMasterGeneralDTO>, int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            //var companyId = _ipAddressService.GetCompanyId();
            //var unitId = _ipAddressService.GetUnitId();
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@PageNumber", PageNumber);
            parameters.Add("@PageSize", PageSize);
            parameters.Add("@SearchTerm", string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm);
            using var multiResult = await _dbConnection.QueryMultipleAsync(
            "dbo.FAM_GetAllAssets", parameters, commandType: CommandType.StoredProcedure);            
            // Read the first result set (Paginated Asset List)
            var assetMasterList = (await multiResult.ReadAsync<AssetMasterGeneralDTO>()).ToList();
            // Read the second result set (Total Record Count)
            int totalCount = await multiResult.ReadFirstAsync<int>();
            // Deserialize JSON for Specifications
            foreach (var asset in assetMasterList)
            {
                if (!string.IsNullOrEmpty(asset.SpecificationsJson))
                {
                    asset.Specifications = JsonConvert.DeserializeObject<List<AssetSpecificationDTO>>(asset.SpecificationsJson);
                }
                else
                {
                    asset.Specifications = new List<AssetSpecificationDTO>();
                }
            }
            return (assetMasterList, totalCount);          
        }
        public async Task<List<AssetMasterGeneralDTO>> GetByAssetNameAsync(string searchPattern)
        {
            const string query = @"            
            SELECT AM.Id,AM.CompanyId,AM.UnitId,AM.AssetCode,AM.AssetName,AM.AssetGroupId,AM.AssetCategoryId,AM.AssetSubCategoryId,AM.AssetParentId,AM.AssetType,AM.MachineCode,AM.Quantity
            ,AM.UOMId,AM.AssetDescription,AM.WorkingStatus,AM.AssetImage,AM.ISDepreciated,AM.IsTangible,AM.IsActive
            ,AM.CreatedBy,AM.CreatedDate,AM.CreatedByName,AM.CreatedIP,AM.ModifiedBy,AM.ModifiedDate,AM.ModifiedByName,AM.ModifiedIP
            ,AG.GroupName AssetGroupName,AC.CategoryName AssetCategoryDesc,A.Description AssetSubCategoryDesc,U.UOMName,MM.description WorkingStatusDesc,M.description AssetTypeDesc,isnull(AM1.AssetDescription,'') ParentAssetDesc
            FROM FixedAsset.AssetMaster AM
            INNER JOIN FixedAsset.AssetGroup AG on AG.Id=AM.AssetGroupId
            INNER JOIN FixedAsset.AssetCategories AC on AC.Id=AM.AssetCategoryId
            INNER JOIN FixedAsset.AssetSubCategories A on A.Id=AM.AssetSubCategoryId
            INNER JOIN FixedAsset.UOM U on U.Id=AM.UOMId
            INNER JOIN FixedAsset.MiscMaster MM on MM.Id =AM.WorkingStatus
            LEFT JOIN FixedAsset.MiscMaster M on M.Id =AM.AssetType
            LEFT JOIN FixedAsset.AssetMaster AM1 on AM1.Id =AM.AssetParentId
            WHERE AM.CompanyId = @CompanyId AND AM.UnitId = @UnitId AND  (AM.AssetName LIKE @SearchPattern OR AM.AssetCode LIKE @SearchPattern) 
            AND  AM.IsDeleted=0 and AM.IsActive=1 ORDER BY AM.ID DESC";            
            //var result = await _dbConnection.QueryAsync<AssetMasterGeneralDTO>(query, new { SearchPattern = $"%{searchPattern}%" });
             var result = await _dbConnection.QueryAsync<AssetMasterGeneralDTO>(
                query,
                new
                {
                    CompanyId,
                    UnitId,
                    SearchPattern = $"%{searchPattern}%"
                });
            return result.ToList();
        }

       public async Task<AssetMasterGeneralDTO> GetByIdAsync(int assetId)
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
            WHERE AM.CompanyId = @CompanyId AND AM.UnitId = @UnitId AND   AM.Id = @assetId AND AM.IsDeleted = 0";

            //var assetMaster = await _dbConnection.<AssetMasterGeneralDTO>(query, new { assetId });
             var assetMaster = await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGeneralDTO>(
            query,
            new
            {
                CompanyId,
                UnitId,
                assetId = $"%{assetId}%"
            });

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
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWorkingStatusAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_WorkingStatus.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetAssetTypeAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_AssetType.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<bool> GetAssetChildDetails(int assetId)
        {
            const string query = @"
                    SELECT 1 FROM [FixedAsset].[AssetLocation] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetPurchaseDetails] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetWarranty] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetSpecifications] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetAmc] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetInsurance] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetAdditionalCost] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetDisposal] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[DepreciationDetail] WHERE AssetId = @Id ";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = assetId });
                    
            var locationExists = await multi.ReadFirstOrDefaultAsync<int?>();  
            var purchaseExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var warrantyExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var specExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var amcExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var insuranceExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var additionalCostExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var depreciationExists = await multi.ReadFirstOrDefaultAsync<int?>();
        
            return locationExists.HasValue || purchaseExists.HasValue || warrantyExists.HasValue  || specExists.HasValue  || amcExists.HasValue || insuranceExists.HasValue || additionalCostExists.HasValue || depreciationExists.HasValue ; 
        }

        public async Task<string?> GetLatestAssetCode( int assetGroupId, int assetCategoryId, int DepartmentId, int LocationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@GroupId", assetGroupId);
            parameters.Add("@CategoryId", assetCategoryId);
            parameters.Add("@DeptId", DepartmentId);
            parameters.Add("@LocationId", LocationId);
            var newAssetCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.FAM_GetAssetCode", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
            return newAssetCode; 
        }

        public async Task<string> GetBaseDirectoryAsync()
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.FAM_GetBaseDirectory", 
                commandType: CommandType.StoredProcedure);
            return result ?? string.Empty; // return an empty string if result is null
        }

        public async  Task<List<Core.Domain.Entities.MiscMaster>>GetAssetPattern()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP,MiscTypeCode
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
              var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_CodePattern.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();        
        }

        public async Task<AssetMasterGeneralDTO> GetByParentIdAsync(int assetTypeId)
        {
              const string query = @"            
            SELECT AM.Id, AM.AssetCode, AM.AssetName
            FROM FixedAsset.AssetMaster AM           
            WHERE AM.Id = @depGroupId AND AM.IsDeleted = 0";

            var assetMaster = await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGeneralDTO>(query, new { assetTypeId });

            if (assetMaster is null)
            {
                throw new KeyNotFoundException($"DepreciationGroup with ID {assetTypeId} not found.");
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
        }

        public async Task<(dynamic AssetResult, dynamic LocationResult, IEnumerable<dynamic> PurchaseDetails, IEnumerable<dynamic> Spec, IEnumerable<dynamic> Warranty, IEnumerable<dynamic> Amc, dynamic Disposal, IEnumerable<dynamic> Insurance, IEnumerable<dynamic> AdditionalCost)> GetAssetMasterByIdAsync(int assetId)
        {
            var sqlQuery = @"
                -- First Query: AssetMaster (One-to-One)
                SELECT AM.AssetName, AM.AssetCode, AM.Quantity, U.UOMName, AG.GroupName,AC.CategoryName, ASUBC.SubCategoryName, AssetParent.AssetName,AM.AssetGroupId ,
                MM.Description+'\'+C.CompanyName+'\'+trim(UN.unitname) +'\'+AM.AssetImage AssetImage,AM.AssetCategoryId,AM.AssetSubCategoryId,
                AM.AssetParentId,AM.AssetType,AM.UOMId,AM.WorkingStatus,AM.AssetImage AssetImageName
                FROM [FixedAsset].[AssetMaster] AM
                INNER JOIN [FixedAsset].[UOM] U ON U.Id = AM.UOMId
                INNER JOIN [FixedAsset].[AssetGroup] AG ON AM.AssetGroupId = AG.Id
                INNER JOIN [FixedAsset].[AssetCategories] AC ON AM.AssetCategoryId = AC.Id
                INNER JOIN [FixedAsset].[AssetSubCategories] ASUBC ON AM.AssetSubCategoryId = ASUBC.Id
                LEFT JOIN [FixedAsset].[AssetMaster] AssetParent ON AM.AssetParentId = AssetParent.Id
                LEFT JOIN FixedAsset.MiscTypeMaster MM on MM.MiscTypeCode ='GETASSETIMAGE'
                LEFT JOIN Bannari.AppData.Unit UN on UN.Id=AM.UnitId
                LEFT JOIN Bannari.AppData.Company C on C.Id=AM.CompanyId
                WHERE  AM.CompanyId = @CompanyId AND AM.UnitId = @UnitId AND   AM.Id = @AssetId;

                -- Second Query: AssetLocation (One-to-One)
                SELECT U.UnitName,D.DeptName,L.LocationName,SL.SubLocationName,U.OldUnitId,AL.CustodianId,AL.UserId,AL.DepartmentId,AL.LocationId,AL.SubLocationId
                FROM [FixedAsset].[AssetLocation] AL
                INNER JOIN [FixedAsset].[Location] L ON L.Id=AL.LocationId
                INNER JOIN [FixedAsset].[SubLocation] SL ON SL.Id=AL.SubLocationId
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AL.UnitId = U.Id
                LEFT JOIN [Bannari].[AppData].[Department] D ON AL.DepartmentId=D.Id                
                WHERE AL.AssetId = @AssetId;
                

                -- Third Query: AssetPurchaseDetails (One-to-Many)
                SELECT distinct AP.Id,AP.VendorCode, AP.VendorName,U.UnitName,ASource.SourceName,AP.GrnNo,Cast(AP.GrnDate AS date) AS GrnDate ,
                AP.GrnSno,AP.GrnValue,AP.PoNo,Cast(AP.PoDate AS date) AS PoDate,AP.PurchaseValue,AP.AcceptedQty,AP.Uom,
                AP.PoSno,AP.ItemCode,AP.ItemName,AP.BillNo,Cast(AP.BillDate AS date) AS BillDate ,AP.BinLocation 
                ,AP.PjYear,AP.PjDocId,AP.PjDocSr,AP.PjDocNo,AP.AssetSourceId ,cast(AP.CapitalizationDate as date)CapitalizationDate
                FROM [FixedAsset].[AssetPurchaseDetails] AP
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AP.OldUnitId = U.OldUnitId
                INNER JOIN [FixedAsset].[AssetSource] ASource ON ASource.Id=AP.AssetSourceId
                WHERE AP.AssetId = @AssetId;

                SELECT A.Id,SM.SpecificationName,A.SpecificationValue,A.SpecificationId,SM.IsDefault FROM  [FixedAsset].[AssetSpecifications] A
                INNER JOIN [FixedAsset].[SpecificationMaster] SM ON SM.Id=A.SpecificationId
                WHERE A.AssetId=@AssetId

                SELECT Aw.Id,CAST(AW.StartDate AS DATE) AS StartDate,CAST(AW.EndDate AS DATE) AS EndDate,AW.Period,MMWaranty.description AS WarrantyType,MMClaim.description AS ServiceClaimStatus,
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

                SELECT AA.Id,CAST(AA.StartDate AS DATE) AS StartDate,CAST(AA.EndDate AS DATE) AS EndDate,AA.Period,AA.VendorCode,AA.VendorName,
                MMCoverage.description AS CoverageType,
                MMRenewal.description AS RenewalStatus,CAST(AA.RenewedDate AS DATE) AS RenewedDate,AA.CoverageType AS CoverageTypeId,
                AA.RenewalStatus AS RenewalStatusId,AA.IsActive,AA.FreeServiceCount,AA.VendorEmail,AA.VendorPhone
                FROM [FixedAsset].[AssetAmc] AA
                INNER JOIN [FixedAsset].[MiscMaster] MMCoverage ON MMCoverage.Id=AA.CoverageType
                INNER JOIN [FixedAsset].[MiscMaster] MMRenewal ON MMRenewal.Id=AA.RenewalStatus
                WHERE AA.AssetId=@AssetId

                SELECT AD.Id,MMDisposal.description AS DisposalType,CAST(AD.DisposalDate AS DATE) AS DisposalDate,AD.DisposalReason,
                AD.DisposalAmount,AD.DisposalType AS DisposalTypeId  ,AD.AssetPurchaseId
                FROM [FixedAsset].[AssetDisposal] AD
                INNER JOIN [FixedAsset].[MiscMaster] MMDisposal ON MMDisposal.Id=AD.DisposalType
                WHERE AD.AssetId=@AssetId

                SELECT Id, PolicyNo,CAST(StartDate AS DATE) AS StartDate,CAST(EndDate AS DATE) AS EndDate,Insuranceperiod,PolicyAmount,
                VendorCode,RenewalStatus,CAST(RenewedDate AS DATE) AS RenewedDate,IsActive
                FROM [FixedAsset].[AssetInsurance]
                WHERE AssetId=@AssetId

                SELECT AC.Id,AssetSourceId,Amount,JournalNo,CostType,MM.Code CostTypeDesc
                FROM [FixedAsset].[AssetAdditionalCost]AC
                inner join FixedAsset.MiscMaster MM on MM.id=CostType 
                WHERE AssetId=@AssetId
                ";

            //using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { AssetId = assetId });

            using var multi = await _dbConnection.QueryMultipleAsync(
                sqlQuery,
                new
                {
                    CompanyId,
                    UnitId,
                    AssetId = assetId
                });            


            var assetResult     = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var locationResult  = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var purchaseDetails = (await multi.ReadAsync<dynamic>()).ToList();
            var specDetails     = (await multi.ReadAsync<dynamic>()).ToList();
            var warrantyDetails = (await multi.ReadAsync<dynamic>()).ToList();
            var amcDetails      = (await multi.ReadAsync<dynamic>()).ToList();
            var disposalResult  = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var insuranceDetails = (await multi.ReadAsync<dynamic>()).ToList();
            var additionalCost   = (await multi.ReadAsync<dynamic>()).ToList();

       
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

            return (assetResult, locationResult, purchaseDetails, specDetails, warrantyDetails, amcDetails, disposalResult, insuranceDetails,additionalCost);
       
        }

        public async Task<(dynamic AssetResult, dynamic LocationResult, IEnumerable<dynamic> PurchaseDetails, IEnumerable<dynamic> AdditionalCost)> GetAssetMasterSplitByIdAsync(int assetId)
        {
           var sqlQuery = @"
                -- First Query: AssetMaster (One-to-One)
                SELECT AM.AssetName, AM.AssetCode, AM.Quantity, U.UOMName, AG.GroupName,AC.CategoryName, ASUBC.SubCategoryName, AssetParent.AssetName,AM.AssetGroupId ,
                MM.Description+'\'+C.CompanyName+'\'+trim(UN.unitname) +'\'+AM.AssetImage AssetImage,AM.AssetCategoryId,AM.AssetSubCategoryId,
                AM.AssetParentId,AM.AssetType,AM.UOMId,AM.WorkingStatus,AM.AssetImage AssetImageName
                FROM [FixedAsset].[AssetMaster] AM
                INNER JOIN [FixedAsset].[UOM] U ON U.Id = AM.UOMId
                INNER JOIN [FixedAsset].[AssetGroup] AG ON AM.AssetGroupId = AG.Id
                INNER JOIN [FixedAsset].[AssetCategories] AC ON AM.AssetCategoryId = AC.Id
                INNER JOIN [FixedAsset].[AssetSubCategories] ASUBC ON AM.AssetSubCategoryId = ASUBC.Id
                LEFT JOIN [FixedAsset].[AssetMaster] AssetParent ON AM.AssetParentId = AssetParent.Id
                LEFT JOIN FixedAsset.MiscTypeMaster MM on MM.MiscTypeCode ='GETASSETIMAGE'
                LEFT JOIN Bannari.AppData.Unit UN on UN.Id=AM.UnitId
                LEFT JOIN Bannari.AppData.Company C on C.Id=AM.CompanyId
                WHERE AM.CompanyId = @CompanyId AND AM.UnitId = @UnitId AND AM.Id = @AssetId;

                -- Second Query: AssetLocation (One-to-One)
                SELECT U.UnitName,D.DeptName,L.LocationName,SL.SubLocationName,U.OldUnitId,AL.CustodianId,AL.UserId,AL.DepartmentId,AL.LocationId,AL.SubLocationId
                FROM [FixedAsset].[AssetLocation] AL
                INNER JOIN [FixedAsset].[Location] L ON L.Id=AL.LocationId
                INNER JOIN [FixedAsset].[SubLocation] SL ON SL.Id=AL.SubLocationId
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AL.UnitId = U.Id
                LEFT JOIN [Bannari].[AppData].[Department] D ON AL.DepartmentId=D.Id                
                WHERE AL.AssetId = @AssetId;
                

                -- Third Query: AssetPurchaseDetails (One-to-Many)
                SELECT distinct AP.Id,AP.VendorCode, AP.VendorName,U.UnitName,ASource.SourceName,AP.GrnNo,Cast(AP.GrnDate AS date) AS GrnDate ,
                AP.GrnSno,AP.GrnValue,AP.PoNo,Cast(AP.PoDate AS date) AS PoDate,AP.PurchaseValue,AP.AcceptedQty,AP.Uom,
                AP.PoSno,AP.ItemCode,AP.ItemName,AP.BillNo,Cast(AP.BillDate AS date) AS BillDate ,AP.BinLocation 
                ,AP.PjYear,AP.PjDocId,AP.PjDocSr,AP.PjDocNo,AP.AssetSourceId ,cast(AP.CapitalizationDate as date)CapitalizationDate
                FROM [FixedAsset].[AssetPurchaseDetails] AP
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AP.OldUnitId = U.OldUnitId
                INNER JOIN [FixedAsset].[AssetSource] ASource ON ASource.Id=AP.AssetSourceId
                WHERE AP.AssetId = @AssetId;             

                SELECT AC.Id,AssetSourceId,Amount,JournalNo,CostType,MM.Code CostTypeDesc
                FROM [FixedAsset].[AssetAdditionalCost]AC
                inner join FixedAsset.MiscMaster MM on MM.id=CostType 
                WHERE AssetId=@AssetId
                ";

            //using var multi = await _dbConnection.QueryMultipleAsync(sqlQuery, new { AssetId = assetId });

            using var multi = await _dbConnection.QueryMultipleAsync(
                sqlQuery,
                new
                {
                    CompanyId,
                    UnitId,
                   AssetId = assetId
                });       



            var assetResult     = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var locationResult  = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
            var purchaseDetails = (await multi.ReadAsync<dynamic>()).ToList();
            var additionalCost   = (await multi.ReadAsync<dynamic>()).ToList();


       
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

            return (assetResult, locationResult, purchaseDetails, additionalCost);
       
        }
        public async Task<(string CompanyName, string UnitName)> GetCompanyUnitAsync(int companyId,int unitId)
        {
            const string query = @"
                SELECT CompanyName 
                FROM Bannari.AppData.Company 
                WHERE Id = @CompanyId;

                SELECT UnitName  
                FROM Bannari.AppData.Unit 
                WHERE Id = @UnitId;
            ";
            using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { CompanyId = companyId, UnitId = unitId });

            var companyName = (await multiQuery.ReadFirstOrDefaultAsync<string>())?.Trim();
            var unitName = (await multiQuery.ReadFirstOrDefaultAsync<string>())?.Trim();

            return (companyName, unitName);
        }  
    }
}