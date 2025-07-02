using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetCustodian;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByCustodian;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetCategoryByDeptId;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetTransferType;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetTransferIssue
{
    public class AssetTransferQueryRepository : IAssetTransferQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _iPAddressService;

        public AssetTransferQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection;
            _iPAddressService = iPAddressService;
        }

        public async Task<(List<AssetTransferDto>, int)> GetAllAsync(int PageNumber, int PageSize, string? SearchTerm, DateTimeOffset? FromDate, DateTimeOffset? ToDate)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetTransferIssueHdr A
                INNER JOIN Bannari.AppData.Unit FromUnit ON A.FromUnitId = FromUnit.Id
                INNER JOIN Bannari.AppData.Unit ToUnit ON A.ToUnitId = ToUnit.Id
                INNER JOIN Bannari.AppData.Department FromDept ON A.FromDepartmentId = FromDept.Id
                INNER JOIN Bannari.AppData.Department ToDept ON A.ToDepartmentId = ToDept.Id
                INNER JOIN FixedAsset.MiscMaster Misc ON A.TransferType = Misc.Id
                WHERE 1 = 1 AND A.FromUnitId = @UnitId
                {{(FromDate.HasValue ? "AND A.DocDate >= @FromDate" : "")}}
                {{(ToDate.HasValue ? "AND A.DocDate <= @ToDate" : "")}}
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search)")}};


                SELECT 
                    A.Id, 
                    A.DocDate, 
                    A.TransferType, 
                    Misc.code as TransferTypeName, 
                    A.FromUnitId, 
                    FromUnit.UnitName AS FromUnitName, 
                    A.ToUnitId, 
                    ToUnit.UnitName AS ToUnitName,
                    A.FromDepartmentId, 
                    FromDept.DeptName AS FromDepartmentName, 
                    A.ToDepartmentId, 
                    ToDept.DeptName AS ToDepartmentName, 
                    A.FromCustodianId, 
                    A.ToCustodianId, 
                    A.Status,  
                    A.FromCustodianName, 
                    A.ToCustodianName, 
                    A.AckStatus, 
                    A.CreatedBy, 
                    A.CreatedDate, 
                    A.CreatedByName, 
                    A.CreatedIP, 
                    A.ModifiedBy, 
                    A.ModifiedDate,
                    A.ModifiedByName,
                    A.ModifiedIP, 
                    A.AuthorizedBy, 
                    A.AuthorizedDate, 
                    A.AuthorizedByName, 
                    A.AuthorizedIP,
                    A.GatePassNo
                FROM FixedAsset.AssetTransferIssueHdr A
                INNER JOIN Bannari.AppData.Unit FromUnit ON A.FromUnitId = FromUnit.Id
                INNER JOIN Bannari.AppData.Unit ToUnit ON A.ToUnitId = ToUnit.Id
                INNER JOIN Bannari.AppData.Department FromDept ON A.FromDepartmentId = FromDept.Id
                INNER JOIN Bannari.AppData.Department ToDept ON A.ToDepartmentId = ToDept.Id
                INNER JOIN FixedAsset.MiscMaster Misc ON A.TransferType = Misc.Id
                WHERE 1 = 1      AND A.FromUnitId = @UnitId    
                {{(FromDate.HasValue ? "AND A.DocDate >= @FromDate" : "")}}
                {{(ToDate.HasValue ? "AND A.DocDate < @ToDate" : "")}}
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search)")}}        
                ORDER BY A.Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                SELECT @TotalCount AS TotalCount;
                """;
            var parameters = new
            {
                UnitId,
                FromDate,
                ToDate = ToDate?.Date.AddDays(1),
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };


            var assetTransfers = await _dbConnection.QueryMultipleAsync(query, parameters);
            var assetTransferList = (await assetTransfers.ReadAsync<AssetTransferDto>()).ToList();
            int totalCount = await assetTransfers.ReadFirstAsync<int>();


            return (assetTransferList, totalCount);
        }
        public async Task<AssetTransferJsonDto> GetAssetTransferByIdAsync(int assetTransferId)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"
            SELECT Id as AssetTransferId , DocDate, TransferType, FromUnitId, ToUnitId, FromDepartmentId, ToDepartmentId, 
                   FromCustodianId, ToCustodianId, Status, FromCustodianName, ToCustodianName,GatePassNo
          
            FROM FixedAsset.AssetTransferIssueHdr
            WHERE Id = @AssetTransferId AND Status = 'Pending'  AND FromUnitId = @UnitId
            FOR JSON PATH, INCLUDE_NULL_VALUES;

            SELECT AssetId, AssetValue 
            FROM FixedAsset.AssetTransferIssueDtl
            WHERE AssetTransferId = @AssetTransferId  
            FOR JSON PATH, INCLUDE_NULL_VALUES;
        ";

            using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { AssetTransferId = assetTransferId, UnitId });

            string headerJson = await multiQuery.ReadFirstOrDefaultAsync<string>();
            string detailsJson = await multiQuery.ReadFirstOrDefaultAsync<string>();

            if (string.IsNullOrWhiteSpace(headerJson))
            {
                return null;
            }

            var header = JsonSerializer.Deserialize<List<AssetTransferJsonDto>>(headerJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })?.FirstOrDefault();

            var details = JsonSerializer.Deserialize<List<AssetTransferDetailJsonDto>>(detailsJson ?? "[]", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (header != null)
            {
                header.AssetTransferDetails = details ?? new List<AssetTransferDetailJsonDto>();
            }

            return header;
        }

        public async Task<List<GetCategoryByDeptIdDto>> GetCategoriesByDepartmentAsync(int departmentId)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"SELECT DISTINCT 
            A.Id AS CategoryID,  A.CategoryName  FROM FixedAsset.AssetCategories A 
            INNER JOIN FixedAsset.AssetMaster   B   ON A.Id = B.AssetCategoryId 
            INNER JOIN FixedAsset.AssetLocation C   ON B.Id = C.AssetId 
            WHERE C.DepartmentId = @departmentId AND B.CompanyId = @CompanyId AND B.UnitId = @UnitId";
            var result = await _dbConnection.QueryAsync<GetCategoryByDeptIdDto>(query, new { departmentId, CompanyId, UnitId });
            return result.ToList();
        }

        public async Task<List<GetCategoryByCustodianDto>> GetCategoryByCustodianAsync(string custodianId, int departmentId)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"SELECT DISTINCT 
            A.Id AS CategoryID,  A.CategoryName  FROM FixedAsset.AssetCategories A 
            INNER JOIN FixedAsset.AssetMaster   B   ON A.Id = B.AssetCategoryId 
            INNER JOIN FixedAsset.AssetLocation C   ON B.Id = C.AssetId 
            WHERE C.CustodianId = @custodianId AND C.DepartmentId = @departmentId AND B.CompanyId = @CompanyId AND B.UnitId = @UnitId";
            var result = await _dbConnection.QueryAsync<GetCategoryByCustodianDto>(query, new { custodianId, departmentId, CompanyId, UnitId });
            return result.ToList();

        }



        public async Task<List<GetAssetCustodianDto>> GetCustodianByDepartmentAsync(string oldUnitId, int departmentId)
        {
            var UnitId = _iPAddressService.GetUnitId();
            // Step 1: Get distinct CustodianIds
            const string custodianQuery = @"
                SELECT DISTINCT CU.CustodianId 
                FROM FixedAsset.AssetLocation CU 
                WHERE CU.DepartmentId = @departmentId 
                AND CU.CustodianId IS NOT NULL 
                AND CU.UnitId = @UnitId";

            var custodianIds = await _dbConnection.QueryAsync<string>(custodianQuery, new { departmentId, UnitId });

            if (!custodianIds.Any())
                return new List<GetAssetCustodianDto>();
            var custodianIdList = string.Join(",", custodianIds);
            var parameters = new DynamicParameters();
            parameters.Add("@DivCode", oldUnitId);
            parameters.Add("@EmpNo", custodianIdList);

            var employeeList = await _dbConnection.QueryAsync<GetAssetCustodianDto>(
                "dbo.GetEmployeeByDivision_ForAssetTransfer",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return employeeList.ToList();
        }





        public async Task<List<GetAssetMasterDto>> GetAssetsByCategoryAsync(int assetCategoryId, int assetDepartmentId)
        {
            // const string query = @"SELECT Id as AssetId, AssetName FROM FixedAsset.AssetMaster WHERE AssetCategoryId = @assetCategoryId"; 
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"	SELECT  A.Id AS AssetId,A.AssetName,A.AssetCategoryId FROM FixedAsset.AssetMaster A 
                                    INNER JOIN FixedAsset.AssetLocation B  ON A.Id = B.AssetId  
                                    WHERE      A.AssetCategoryId = @assetCategoryId   AND B.DepartmentId =  @assetDepartmentId  AND A.CompanyId = @CompanyId AND A.UnitId = @UnitId";
            var result = await _dbConnection.QueryAsync<GetAssetMasterDto>(query, new { assetCategoryId, assetDepartmentId, CompanyId, UnitId });
            return result.ToList();
        }

        public async Task<GetAssetDetailsToTransferHdrDto> GetAssetDetailsToTransferByIdAsync(int assetId)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"
                    -- Get Asset Master Details
                    SELECT 
                        A.Id AS AssetId, A.CreatedDate as DocDate,  H.CategoryName, A.AssetCode, A.AssetName, 
                        A.UnitId, G.UnitName, B.LocationId, C.LocationName, B.SubLocationId, D.SubLocationName, 
                        B.DepartmentId, F.DeptName AS DepartmentName
                    FROM FixedAsset.AssetMaster A
                    INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
                    INNER JOIN FixedAsset.Location C ON B.LocationId = C.Id
                    INNER JOIN FixedAsset.SubLocation D ON B.SubLocationId = D.Id
                    INNER JOIN Bannari.AppData.Department F ON B.DepartmentId = F.Id
                    INNER JOIN Bannari.AppData.Unit G ON A.UnitId = G.Id
                    INNER JOIN FixedAsset.AssetCategories H ON A.AssetCategoryId = H.Id
                    WHERE A.Id = @AssetId   AND A.CompanyId = @CompanyId AND A.UnitId = @UnitId
                    FOR JSON PATH, INCLUDE_NULL_VALUES;

                    -- Get Asset Transfer Issue Details
                    SELECT AssetId ,GrnValue as AssetValue
                    FROM FixedAsset.AssetPurchaseDetails 
                    WHERE AssetId = @AssetId
                    FOR JSON PATH, INCLUDE_NULL_VALUES;


                     SELECT U.UnitName,D.DeptName,L.LocationName,SL.SubLocationName,U.OldUnitId,AL.CustodianId,AL.UserId as FromCustodianId ,AL.UserId as ToCustodianId  FROM [FixedAsset].[AssetLocation] AL
                INNER JOIN [FixedAsset].[Location] L ON L.Id=AL.LocationId
                INNER JOIN [FixedAsset].[SubLocation] SL ON SL.Id=AL.SubLocationId
                LEFT JOIN [Bannari].[AppData].[Unit] U ON AL.UnitId = U.Id
                LEFT JOIN [Bannari].[AppData].[Department] D ON AL.DepartmentId=D.Id                
                WHERE AL.AssetId =@AssetId     


                ";

            using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { AssetId = assetId, CompanyId, UnitId });

            string assetJson = await multiQuery.ReadFirstOrDefaultAsync<string>();
            string transferJson = await multiQuery.ReadFirstOrDefaultAsync<string>();
            var location = await multiQuery.ReadFirstOrDefaultAsync<dynamic>();


            if (string.IsNullOrWhiteSpace(assetJson))
            {
                return null; // Asset not found
            }


            var assetDetails = JsonSerializer.Deserialize<List<GetAssetDetailsToTransferHdrDto>>(assetJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })?.FirstOrDefault();

            // Deserialize Transfer Issue Details
            var transferDetails = JsonSerializer.Deserialize<List<GetAssetDetailsToTransferDto>>(transferJson ?? "[]", new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (assetDetails != null)
            {
                assetDetails.GetAssetDetailToTransfer = transferDetails ?? new List<GetAssetDetailsToTransferDto>();

                if (location != null)
                {
                    assetDetails.UnitName = location.UnitName;
                    assetDetails.DepartmentName = location.DeptName;
                    assetDetails.LocationName = location.LocationName;
                    assetDetails.SubLocationName = location.SubLocationName;
                    assetDetails.FromCustodianId = location.CustodianId;
                    assetDetails.OldUnitId = location.OldUnitId;
                    assetDetails.ToCustodianId = location.ToCustodianId;

                    // Fetch Custodian Name if CustodianId is valid;
                    if (location.CustodianId > 0 && !string.IsNullOrEmpty(location.OldUnitId))
                    {
                        var custodianParams = new
                        {
                            DivCode = location.OldUnitId,
                            EmpNo = location.CustodianId
                        };

                        var custodianEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                            "dbo.GetEmployeeByDivision",
                            custodianParams,
                            commandType: CommandType.StoredProcedure
                        );

                        assetDetails.FromCustodianName = custodianEmployee?.Empname;
                    }
                    // Fetch User
                    if (assetDetails.ToCustodianId > 0)
                    {
                        var userParams = new
                        {
                            DivCode = assetDetails.OldUnitId,
                            EmpNo = assetDetails.ToCustodianId
                        };

                        var userEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                            "dbo.GetEmployeeByDivision",
                            userParams,
                            commandType: CommandType.StoredProcedure
                        );

                        if (userEmployee != null)
                            assetDetails.ToCustodianName = userEmployee.Empname;
                    }
                }
            }
            return assetDetails;
        }
        public async Task<bool> IsAssetPendingOrApprovedAsync(int assetId)
        {
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"
                        SELECT 1 FROM FixedAsset.AssetTransferIssueHdr A
                        INNER JOIN FixedAsset.AssetTransferIssueDtl B ON A.Id = B.AssetTransferId
                        WHERE B.AssetId = @assetId  
                        AND (A.Status = 'Pending' OR (A.Status = 'Approved' AND A.AckStatus <> 1))";

            var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(query, new { assetId });
            return result.HasValue; // If record exists, return true (restricted)
        }

        public async Task<List<GetAllTransferDtlDto>> GetAssetTransferByIDAsync(int assetTransferId)
        {
            var CompanyId = _iPAddressService.GetCompanyId();
            var UnitId = _iPAddressService.GetUnitId();
            const string query = @"SELECT  A.Id,A.AssetTransferId,A.AssetId,B.AssetCode,B.AssetName,A.AssetValue  FROM FixedAsset.AssetTransferIssueDtl A 
			                                 INNER JOIN  FixedAsset.AssetMaster B on  A.AssetId=B.ID WHERE AssetTransferId = @assetTransferId AND B.CompanyId = @CompanyId AND B.UnitId = @UnitId";
            var result = await _dbConnection.QueryAsync<GetAllTransferDtlDto>(query, new { assetTransferId, CompanyId, UnitId });
            return result.ToList();
        }


        public async Task<List<Core.Domain.Entities.MiscMaster>> GetTransferTypeAsync()
        {

            const string query = @"
                    SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder, M.IsActive
                    ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
                    FROM FixedAsset.MiscMaster M
                    INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
                    WHERE (MiscTypeCode = @MiscTypeCode)
                    AND M.IsDeleted=0 and M.IsActive=1 
                    ORDER BY M.ID DESC";
            var parameters = new { MiscTypeCode = MiscEnumEntity.AssetTransferType.MiscCode };
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
            return result.ToList();
        }


        public async Task<bool> DepartmentSoftDeleteValidation(int Id)
        {
            const string query = @"
                        SELECT 1 FROM FixedAsset.AssetTransferIssueHdr WHERE FromDepartmentId = @Id;
                        SELECT 1 FROM FixedAsset.AssetTransferIssueHdr WHERE ToDepartmentId = @Id;
                        SELECT 1 FROM FixedAsset.AssetLocation WHERE DepartmentId = @Id;
                        SELECT 1 FROM FixedAsset.SubLocation WHERE DepartmentId = @Id;
                        SELECT 1 FROM FixedAsset.Location WHERE DepartmentId = @Id AND IsDeleted = 0;
                    ";

            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id });

            var fromIssueExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var toIssueExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var assetLocationExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var subLocationExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var locationExists = await multi.ReadFirstOrDefaultAsync<int?>();

            return fromIssueExists.HasValue ||
                toIssueExists.HasValue ||
                assetLocationExists.HasValue ||
                subLocationExists.HasValue ||
                locationExists.HasValue;
        }
                public async Task<List<GetAssetDetailsToTransferHdrDto>> GetAssetDetailsToTransferByFiltersAsync(
            string custodianIdsCsv, int departmentId, string categoryIdsCsv)
        {
            var companyId = _iPAddressService.GetCompanyId();
            var unitId = _iPAddressService.GetUnitId();

            var sql = @"
                DECLARE @PendingStatus VARCHAR(50) = 'Pending';
                DECLARE @ApprovedStatus VARCHAR(50) = 'Approved';

                DECLARE @AssetIds TABLE (AssetId INT);

                INSERT INTO @AssetIds (AssetId)
                SELECT A.Id
                FROM FixedAsset.AssetMaster A
                INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
                WHERE A.UnitId = @UnitId
                AND B.DepartmentId = @DepartmentId
                AND A.AssetCategoryId IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@AssetCategoryIds, ','))
                AND B.CustodianId IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@CustodianIds, ','))
                AND NOT EXISTS (
                    SELECT 1
                    FROM FixedAsset.AssetTransferIssueHdr h
                    INNER JOIN FixedAsset.AssetTransferIssueDtl d ON h.Id = d.AssetTransferId
                    WHERE d.AssetId = A.Id
                        AND h.Status = @PendingStatus
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM FixedAsset.AssetTransferIssueHdr h
                    INNER JOIN FixedAsset.AssetTransferIssueDtl d ON h.Id = d.AssetTransferId
                    LEFT JOIN FixedAsset.AssetTransferReceiptDtl r ON d.AssetId = r.AssetId
                    WHERE d.AssetId = A.Id
                        AND h.Status = @ApprovedStatus
                        AND (r.AckStatus = 0 OR r.AckStatus IS NULL)
                );

                SELECT  
                    A.Id AS AssetId, 
                    A.CreatedDate as DocDate,  
                    H.CategoryName, 
                    A.AssetCode, 
                    A.AssetName, 
                    A.UnitId, 
                    G.UnitName, 
                    B.LocationId, 
                    C.LocationName, 
                    B.SubLocationId, 
                    D.SubLocationName, 
                    B.DepartmentId, 
                    F.DeptName AS DepartmentName,
                    NULL AS FromCustodianId,        -- placeholder, will override from location data below
                    NULL AS ToCustodianId,
                    NULL AS OldUnitId
                FROM FixedAsset.AssetMaster A
                INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
                INNER JOIN FixedAsset.Location C ON B.LocationId = C.Id
                INNER JOIN FixedAsset.SubLocation D ON B.SubLocationId = D.Id
                INNER JOIN Bannari.AppData.Department F ON B.DepartmentId = F.Id
                INNER JOIN Bannari.AppData.Unit G ON A.UnitId = G.Id
                INNER JOIN FixedAsset.AssetCategories H ON A.AssetCategoryId = H.Id
                WHERE A.Id IN (SELECT AssetId FROM @AssetIds);

                SELECT AssetId, GrnValue AS AssetValue
                FROM FixedAsset.AssetPurchaseDetails 
                WHERE AssetId IN (SELECT AssetId FROM @AssetIds);

                SELECT 
                    AL.AssetId,
                    U.UnitName,
                    D.DeptName,
                    L.LocationName,
                    SL.SubLocationName,
                    U.OldUnitId,
                    AL.CustodianId AS FromCustodianId,
                    AL.UserId AS ToCustodianId
                FROM FixedAsset.AssetLocation AL
                INNER JOIN FixedAsset.Location L ON L.Id = AL.LocationId
                INNER JOIN FixedAsset.SubLocation SL ON SL.Id = AL.SubLocationId
                LEFT JOIN Bannari.AppData.Unit U ON AL.UnitId = U.Id
                LEFT JOIN Bannari.AppData.Department D ON AL.DepartmentId = D.Id                
                WHERE AL.AssetId IN (SELECT AssetId FROM @AssetIds);
            ";

            using var multi = await _dbConnection.QueryMultipleAsync(sql, new
            {
                UnitId = unitId,
                DepartmentId = departmentId,
                AssetCategoryIds = categoryIdsCsv,
                CustodianIds = custodianIdsCsv
            });

            var assetDynamics = (await multi.ReadAsync<dynamic>()).ToList();
            var purchaseDynamics = (await multi.ReadAsync<dynamic>()).ToList();
            var locationDynamics = (await multi.ReadAsync<dynamic>()).ToList();

            // Map assets
            var assets = assetDynamics.Select(a => new GetAssetDetailsToTransferHdrDto
            {
                AssetID = a.AssetId,
                DocDate = a.DocDate,
                CategoryName = a.CategoryName,
                AssetCode = a.AssetCode,
                AssetName = a.AssetName,
                UnitId = a.UnitId,
                UnitName = a.UnitName,
                LocationId = a.LocationId,
                LocationName = a.LocationName,
                SubLocationId = a.SubLocationId,
                SubLocationName = a.SubLocationName,
                DepartmentId = a.DepartmentId,
                DepartmentName = a.DepartmentName,
                FromCustodianId = 0, // will assign below
                ToCustodianId = 0,
                OldUnitId = null,
                FromCustodianName = null,
                ToCustodianName = null,
                GetAssetDetailToTransfer = new List<GetAssetDetailsToTransferDto>()
            }).ToList();

            // Map purchase details
            var purchaseDetails = purchaseDynamics.Select(p => new GetAssetDetailsToTransferDto
            {
                AssetId = p.AssetId,
                AssetValue = p.AssetValue
            }).ToList();

            var purchaseDict = purchaseDetails.GroupBy(p => p.AssetId)
                                            .ToDictionary(g => g.Key, g => g.ToList());

            // Map location details by AssetId
            var locationDict = locationDynamics.ToDictionary(l => (int)l.AssetId);

            // Update assets with location and custodian info
            foreach (var asset in assets)
            {
                // Assign purchase details
                if (purchaseDict.TryGetValue(asset.AssetID, out var purchases))
                {
                    asset.GetAssetDetailToTransfer = purchases;
                }

                // Assign location info if available
                if (locationDict.TryGetValue(asset.AssetID, out var location))
                {
                    asset.UnitName = location.UnitName;
                    asset.DepartmentName = location.DeptName;
                    asset.LocationName = location.LocationName;
                    asset.SubLocationName = location.SubLocationName;
                    asset.OldUnitId = location.OldUnitId;
                    asset.FromCustodianId = location.FromCustodianId ?? 0;
                    asset.ToCustodianId = location.ToCustodianId ?? 0;
                }

                // Fetch FromCustodianName
                if (asset.FromCustodianId > 0 && !string.IsNullOrEmpty(asset.OldUnitId))
                {
                    var custodianParams = new
                    {
                        DivCode = asset.OldUnitId,
                        EmpNo = asset.FromCustodianId
                    };

                    var custodianEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                        "dbo.GetEmployeeByDivision",
                        custodianParams,
                        commandType: CommandType.StoredProcedure
                    );

                    asset.FromCustodianName = custodianEmployee?.Empname;
                }

                // Fetch ToCustodianName
                if (asset.ToCustodianId > 0 && !string.IsNullOrEmpty(asset.OldUnitId))
                {
                    var userParams = new
                    {
                        DivCode = asset.OldUnitId,
                        EmpNo = asset.ToCustodianId
                    };

                    var userEmployee = await _dbConnection.QueryFirstOrDefaultAsync<Employee>(
                        "dbo.GetEmployeeByDivision",
                        userParams,
                        commandType: CommandType.StoredProcedure
                    );

                    asset.ToCustodianName = userEmployee?.Empname;
                }
            }

            return assets;
        }

        
        // public async Task<List<GetAssetDetailsToTransferHdrDto>> GetAssetDetailsToTransferByFiltersAsync( string custodianIdsCsv, int departmentId, string categoryIdsCsv)
        // {
        //              var companyId = _iPAddressService.GetCompanyId();
        //              var unitId = _iPAddressService.GetUnitId();

        //              var sql = @"
        //          DECLARE @PendingStatus VARCHAR(50) = 'Pending';
        //          DECLARE @ApprovedStatus VARCHAR(50) = 'Approved';

        //          DECLARE @AssetIds TABLE (AssetId INT);

        //          INSERT INTO @AssetIds (AssetId)
        //          SELECT A.Id
        //          FROM FixedAsset.AssetMaster A
        //          INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
        //          WHERE A.UnitId = @UnitId
        //            AND B.DepartmentId = @DepartmentId
        //            AND A.AssetCategoryId IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@AssetCategoryIds, ','))
        //            AND B.CustodianId IN (SELECT CAST(value AS INT) FROM STRING_SPLIT(@CustodianIds, ','))
        //            AND NOT EXISTS (
        //                SELECT 1
        //                FROM FixedAsset.AssetTransferIssueHdr h
        //                INNER JOIN FixedAsset.AssetTransferIssueDtl d ON h.Id = d.AssetTransferId
        //                WHERE d.AssetId = A.Id
        //                  AND h.Status = @PendingStatus
        //            )
        //            AND NOT EXISTS (
        //                SELECT 1
        //                FROM FixedAsset.AssetTransferIssueHdr h
        //                INNER JOIN FixedAsset.AssetTransferIssueDtl d ON h.Id = d.AssetTransferId
        //                LEFT JOIN FixedAsset.AssetTransferReceiptDtl r ON d.AssetId = r.AssetId
        //                WHERE d.AssetId = A.Id
        //                  AND h.Status = @ApprovedStatus
        //                  AND (r.AckStatus = 0 OR r.AckStatus IS NULL)
        //            );

        //          SELECT  
        //              A.Id AS AssetId, 
        //              A.CreatedDate as DocDate,  
        //              H.CategoryName, 
        //              A.AssetCode, 
        //              A.AssetName, 
        //              A.UnitId, 
        //              G.UnitName, 
        //              B.LocationId, 
        //              C.LocationName, 
        //              B.SubLocationId, 
        //              D.SubLocationName, 
        //              B.DepartmentId, 
        //              F.DeptName AS DepartmentName
        //          FROM FixedAsset.AssetMaster A
        //          INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
        //          INNER JOIN FixedAsset.Location C ON B.LocationId = C.Id
        //          INNER JOIN FixedAsset.SubLocation D ON B.SubLocationId = D.Id
        //          INNER JOIN Bannari.AppData.Department F ON B.DepartmentId = F.Id
        //          INNER JOIN Bannari.AppData.Unit G ON A.UnitId = G.Id
        //          INNER JOIN FixedAsset.AssetCategories H ON A.AssetCategoryId = H.Id
        //          WHERE A.Id IN (SELECT AssetId FROM @AssetIds);

        //          SELECT  AssetId, GrnValue AS AssetValue
        //          FROM FixedAsset.AssetPurchaseDetails 
        //          WHERE AssetId IN (SELECT AssetId FROM @AssetIds);

        //                     SELECT 
        //         U.UnitName,
        //         D.DeptName,
        //         L.LocationName,
        //         SL.SubLocationName,
        //         U.OldUnitId,
        //         AL.CustodianId as FromCustodianId,
        //         AL.UserId as ToCustodianId,
        //         AL.AssetId
        //     FROM FixedAsset.AssetLocation AL
        //     INNER JOIN FixedAsset.Location L ON L.Id = AL.LocationId
        //     INNER JOIN FixedAsset.SubLocation SL ON SL.Id = AL.SubLocationId
        //     LEFT JOIN Bannari.AppData.Unit U ON AL.UnitId = U.Id
        //     LEFT JOIN Bannari.AppData.Department D ON AL.DepartmentId = D.Id                
        //     WHERE AL.AssetId IN (SELECT AssetId FROM @AssetIds);
        //      ";

        //      using var multi = await _dbConnection.QueryMultipleAsync(sql, new
        //      {
        //          UnitId = unitId,
        //          DepartmentId = departmentId,
        //          AssetCategoryIds = categoryIdsCsv,
        //          CustodianIds = custodianIdsCsv
        //      });

        //      var assetDynamics  = (await multi.ReadAsync<dynamic>()).ToList();
        //      var purchaseDynamics = (await multi.ReadAsync<dynamic>()).ToList();
        //      var assetLocationDynamics = (await multi.ReadAsync<dynamic>()).ToList();

        //       var assets = assetDynamics.Select(a => new GetAssetDetailsToTransferHdrDto
        //     {
        //         AssetID = a.AssetId,
        //         DocDate = a.DocDate,
        //         CategoryName = a.CategoryName,
        //         AssetCode = a.AssetCode,
        //         AssetName = a.AssetName,
        //         UnitId = a.UnitId,
        //         UnitName = a.UnitName,
        //         LocationId = a.LocationId,
        //         LocationName = a.LocationName,
        //         SubLocationId = a.SubLocationId,
        //         SubLocationName = a.SubLocationName,
        //         DepartmentId = a.DepartmentId,
        //         DepartmentName = a.DepartmentName,
        //         FromCustodianId = a.FromCustodianId == null ? 0 : (int)a.FromCustodianId,
        //         ToCustodianId = a.ToCustodianId == null ? 0 : (int)a.ToCustodianId,               
        //         GetAssetDetailToTransfer = new List<GetAssetDetailsToTransferDto>()
        //     }).ToList();

        //          var purchaseDetails = purchaseDynamics.Select(p => new GetAssetDetailsToTransferDto
        //          {
        //              AssetId = p.AssetId,
        //              AssetValue = p.AssetValue
        //          }).ToList();


        //          var purchaseDict = purchaseDetails.GroupBy(p => p.AssetId)
        //                                            .ToDictionary(g => g.Key, g => g.ToList());


        //          foreach (var asset in assets)
        //          {
        //              if (purchaseDict.TryGetValue(asset.AssetID, out var purchases))
        //              {
        //                  asset.GetAssetDetailToTransfer = purchases;
        //              }
        //          }


        //      return assets;
        //  }

    }

}