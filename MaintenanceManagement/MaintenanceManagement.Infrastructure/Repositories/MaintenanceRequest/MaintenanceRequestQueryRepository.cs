using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.MaintenanceRequest.Queries.GetExternalRequestById;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.RequestReport;
using Core.Domain.Common;
using Core.Domain.Entities;
using Dapper;
using MaintenanceManagement.Infrastructure.Services;

namespace MaintenanceManagement.Infrastructure.Repositories.MaintenanceRequest
{
    public class MaintenanceRequestQueryRepository : IMaintenanceRequestQueryRepository
    {
                private readonly IDbConnection _dbConnection; 

                private readonly IIPAddressService _iPAddressService;

                public MaintenanceRequestQueryRepository(IDbConnection dbConnection , IIPAddressService iPAddressService)
                {
                    _dbConnection = dbConnection;
                    _iPAddressService = iPAddressService;
                }

                public async Task<(IEnumerable<dynamic> MaintenanceRequestList, int)> GetAllMaintenanceRequestAsync(int PageNumber, int PageSize, string? SearchTerm, DateTimeOffset? FromDate,    DateTimeOffset? ToDate   ) 
                {
                    var UnitId= _iPAddressService.GetUnitId();
                                        var query = $$"""
                        DECLARE @TotalCount INT;

                        SELECT @TotalCount = COUNT(*)
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id  
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id  
                         LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id 
                         LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id 

                        WHERE A.IsDeleted = 0
                        AND (@FromDate IS NULL OR A.CreatedDate >= @FromDate)
                        AND (@ToDate IS NULL OR A.CreatedDate <= @ToDate) AND A.UnitId = @UnitId
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search  OR F.Code LIKE @Search OR G.Code LIKE @Search OR E.MachineName LIKE @Search OR H.Code LIKE @Search OR I.Code LIKE @Search OR J.Code LIKE @Search ) ")}};
                                            
                        SELECT 
                            A.Id,
                            A.DepartmentId,
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,

                            A.CreatedByName,
                            A.CreatedDate,
                            A.CreatedBy,
                            A.CreatedIP,
                            A.ModifiedByName,
                            A.ModifiedDate,
                            A.ModifiedBy,
                            A.ModifiedIP,
                            B.Id AS RequestTypeId,
                            B.Code AS RequestType,
                            C.Id AS MaintenanceTypeId,
                            C.Code AS MaintenanceType,
                            E.Id AS MachineId,
                            E.MachineName AS MachineName,
                            F.Id AS ServiceTypeId,
                            F.Code AS ServiceType,
                            Cast(A.ExpectedDispatchDate AS Date) AS ExpectedDispatchDate,
                            G.Id AS ServiceLocationId,
                            G.Code AS ServiceLocation,
                            H.Id AS ModeOfDispatchId,
                            H.Code AS ModeOfDispatch,
                            I.Id AS SparesTypeId,
                            I.Code AS SparesType,
                            J.Id AS RequestStatusId,
                            J.Code AS RequestStatus

                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id 
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id    
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id  
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id 
                        LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id 
                        LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id 

                        WHERE A.IsDeleted = 0   AND  B.Code = @MiscCode  AND J.Code <> @MaintenanceStatusUpdate
                        AND (@FromDate IS NULL OR A.CreatedDate >= @FromDate)
                        AND (@ToDate IS NULL OR A.CreatedDate <= @ToDate)
                        AND A.UnitId = @UnitId
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search OR F.Code LIKE @Search or G.Code LIKE @Search OR H.Code LIKE @Search  OR E.MachineName LIKE @Search OR I.Code LIKE @Search  or J.Code LIKE @Search) ")}}
                        ORDER BY A.Id DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        SELECT @TotalCount AS TotalCount;
                        """;
                    var parameters = new
                    {
                       //  MiscTypeCode = MiscEnumEntity.MaintenanceRequestType.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeInternal.Code,
                        MaintenanceStatusUpdate = MiscEnumEntity.MaintenanceStatusUpdate.Code,
                        Search = $"%{SearchTerm}%",
                        Offset = (PageNumber - 1) * PageSize,
                        PageSize,
                        FromDate = FromDate?.Date,
                        ToDate = ToDate?.Date.AddDays(1).AddTicks(-1),
                        UnitId

                        
                    };

                         using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
                       var maintenanceReqList = await multi.ReadAsync<dynamic>();
                       var totalCount = await multi.ReadFirstAsync<int>();
           

                    return (maintenanceReqList, totalCount);
                }

                // External Request
                 public async Task<(IEnumerable<dynamic> MaintenanceRequestList, int)> GetAllMaintenanceExternalRequestAsync(int PageNumber, int PageSize, string? SearchTerm ,DateTimeOffset? FromDate,  DateTimeOffset? ToDate)
                {

                      var UnitId= _iPAddressService.GetUnitId();
                                        var query = $$"""
                        DECLARE @TotalCount INT;

                        SELECT @TotalCount = COUNT(*)
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id  
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id  
                         LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id 
                         LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id 

                        WHERE A.IsDeleted = 0 AND (@FromDate IS NULL OR A.CreatedDate >= @FromDate)
                        AND (@ToDate IS NULL OR A.CreatedDate <= @ToDate)  AND A.UnitId = @UnitId
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search  OR F.Code LIKE @Search OR G.Code LIKE @Search OR E.MachineName LIKE @Search OR H.Code LIKE @Search OR I.Code LIKE @Search OR J.Code LIKE @Search ) ")}};
                                            
                        SELECT 
                            A.Id,
                            A.DepartmentId,
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,
                            A.EstimatedServiceCost,
                            A.EstimatedSpareCost,
                             A.CreatedByName,
                            A.CreatedDate,
                            A.CreatedBy,
                            A.CreatedIP,
                             A.ModifiedByName,
                            A.ModifiedDate,
                            A.ModifiedBy,
                            A.ModifiedIP,
                            B.Id AS RequestTypeId,
                            B.Code AS RequestType,
                            C.Id AS MaintenanceTypeId,
                            C.Code AS MaintenanceType,
                            E.Id AS MachineId,
                            E.MachineName AS MachineName,
                            F.Id AS ServiceTypeId,
                            F.Code AS ServiceType,
                            Cast(A.ExpectedDispatchDate AS Date) AS ExpectedDispatchDate,
                            G.Id AS ServiceLocationId,
                            G.Code AS ServiceLocation,
                            H.Id AS ModeOfDispatchId,
                            H.Code AS ModeOfDispatch,
                            I.Id AS SparesTypeId,
                            I.Code AS SparesType,
                            J.Id AS RequestStatusId,
                            J.Code AS RequestStatus

                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id 
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id    
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id  
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id 
                        LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id 
                        LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id 

                        WHERE  B.Code = @MiscCode  AND C.Code <> @MaintenanceStatusUpdate 
                        AND (@FromDate IS NULL OR A.CreatedDate >= @FromDate)
                        AND (@ToDate IS NULL OR A.CreatedDate <= @ToDate) AND A.UnitId = @UnitId
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search OR F.Code LIKE @Search or G.Code LIKE @Search OR H.Code LIKE @Search  OR E.MachineName LIKE @Search OR I.Code LIKE @Search  or J.Code LIKE @Search) ")}}
                        ORDER BY A.Id DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        SELECT @TotalCount AS TotalCount;
                        """;
                    var parameters = new
                    {
                       //  MiscTypeCode = MiscEnumEntity.MaintenanceRequestType.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeExternal.Code,
                        MaintenanceStatusUpdate = MiscEnumEntity.MaintenanceStatusUpdate.Code,
                        Search = $"%{SearchTerm}%",
                        Offset = (PageNumber - 1) * PageSize,
                        PageSize,
                        FromDate = FromDate?.Date,
                        ToDate = ToDate?.Date.AddDays(1).AddTicks(-1),
                        UnitId
                    };

                         using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
                       var maintenanceReqList = await multi.ReadAsync<dynamic>();
                       var totalCount = await multi.ReadFirstAsync<int>();
           

                    return (maintenanceReqList, totalCount);
                }


          
                public async Task<dynamic?> GetByIdAsync(int id)
                {

                    var UnitId= _iPAddressService.GetUnitId();
                    var query = @"
                        SELECT 
                            A.Id,
                            A.DepartmentId,
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,
                            A.EstimatedServiceCost,
                            A.EstimatedSpareCost,
                            A.CreatedByName,
                            A.CreatedDate,
                            A.CreatedBy,
                            A.CreatedIP,
                            A.ModifiedByName,
                            A.ModifiedDate,
                            A.ModifiedBy,
                            A.ModifiedIP,
                            B.Id AS RequestTypeId,
                            B.Code AS RequestType,
                            C.Id AS MaintenanceTypeId,
                            C.Code AS MaintenanceType,
                            E.Id AS MachineId,
                            E.MachineName AS MachineName,
                            F.Id AS ServiceTypeId,
                            F.Code AS ServiceType,
                            CAST(A.ExpectedDispatchDate AS DATE) AS ExpectedDispatchDate,
                            G.Id AS ServiceLocationId,
                            G.Code AS ServiceLocation,
                            H.Id AS ModeOfDispatchId,
                            H.Code AS ModeOfDispatch,
                            I.Id AS SparesTypeId,
                            I.Code AS SparesType,
                            J.Id AS RequestStatusId,
                            J.Code AS RequestStatus

                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id
                        LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id
                        LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id

                        WHERE A.IsDeleted = 0 AND A.Id = @Id 
                        AND B.Code = @MiscCode AND A.UnitId = @UnitId;
                    ";

                   // var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(query, new { Id = id });
                    
                     var parameters = new
                    {
                        Id = id,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeInternal.Code,
                         MiscStatusCode = MiscEnumEntity.MaintenanceStatusUpdate.Code,
                        UnitId
                        
                    };

                     var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);

                    return result;
                }

                public async Task<List<GetExternalRequestByIdDto>> GetExternalRequestByIdAsync(List<int> ids)
                {
                    var UnitId= _iPAddressService.GetUnitId();
                    var query = @"
                        SELECT 
                            A.Id,
                            A.DepartmentId,
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,
                            A.CompanyId,
                            A.UnitId,
                             A.EstimatedServiceCost,
                            A.EstimatedSpareCost,
                             A.CreatedByName,
                            A.CreatedDate,
                            A.CreatedBy,
                            A.CreatedIP,
                             A.ModifiedByName,
                            A.ModifiedDate,
                            A.ModifiedBy,
                            A.ModifiedIP,
                            B.Id AS RequestTypeId,
                            B.Code AS RequestType,
                            C.Id AS MaintenanceTypeId,
                            C.Code AS MaintenanceType,
                            E.Id AS MachineId,
                            E.MachineName AS MachineName,
                            F.Id AS ServiceTypeId,
                            F.Code AS ServiceType,
                            CAST(A.ExpectedDispatchDate AS DATE) AS ExpectedDispatchDate,
                            G.Id AS ServiceLocationId,
                            G.Code AS ServiceLocation,
                            H.Id AS ModeOfDispatchId,
                            H.Code AS ModeOfDispatch,
                            I.Id AS SparesTypeId,
                            I.Code AS SparesType,
                            J.Id AS RequestStatusId,
                            J.Code AS RequestStatus
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        LEFT JOIN Maintenance.MiscMaster F ON A.ServiceTypeId = F.Id
                        LEFT JOIN Maintenance.MiscMaster G ON A.ServiceLocationId = G.Id
                        LEFT JOIN Maintenance.MiscMaster H ON A.ModeOfDispatchId = H.Id
                        LEFT JOIN Maintenance.MiscMaster I ON A.SparesTypeId = I.Id
                        LEFT JOIN Maintenance.MiscMaster J ON A.RequestStatusId = J.Id
                        INNER JOIN Maintenance.MiscTypeMaster K ON J.MiscTypeId = K.Id
                        WHERE  A.Id IN @Ids  
                        AND B.Code = @MiscCode AND J.Code <> @MiscStatusCode AND A.UnitId = @UnitId AND K.MiscTypeCode =@MiscType ;
                    ";
                //    var result = await _dbConnection.QueryAsync<GetExternalRequestByIdDto>(query, new { Ids = ids });
                 var parameters = new
                    {
                        Ids = ids,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeExternal.Code,
                        MiscStatusCode = MiscEnumEntity.MaintenanceStatusUpdate.Code,
                        MiscType= MiscEnumEntity.WOStatus.MiscCode,
                        UnitId
                       
                    };

                  //   var result = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(query, parameters);
                   var result = await _dbConnection.QueryAsync<GetExternalRequestByIdDto>(query, parameters);
                    return result.ToList(); // always return a list (empty if nothing found)
                }  
         

                 public async Task<List<Core.Domain.Entities.ExistingVendorDetails>> GetVendorDetails(string OldUnitId, string? VendorCode)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OldUnitId", OldUnitId, DbType.String);

                // Correctly pass NULL or wildcard pattern
                if (!string.IsNullOrWhiteSpace(VendorCode))
                {
                    parameters.Add("@Slcode", VendorCode, DbType.String); // No wildcard in C# - handled in SQL
                }
                else
                {
                    parameters.Add("@Slcode", DBNull.Value, DbType.String);
                }

                var vendorDetailsList = await _dbConnection.QueryAsync<Core.Domain.Entities.ExistingVendorDetails>(
                    "dbo.GetVendorDetails", 
                    parameters, 
                    commandType: CommandType.StoredProcedure
                );

                if (!vendorDetailsList.Any())
                {
                    Console.WriteLine("No data returned from stored procedure!");
                }

                    return vendorDetailsList?.ToList() ?? new List<Core.Domain.Entities.ExistingVendorDetails>();
            }


               public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenancestatusAsync()
                {
                    const string query = @"
                        SELECT M.Id, MiscTypeId, Code, M.Description, SortOrder, M.IsActive,
                            M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP,
                            M.ModifiedBy, M.ModifiedDate, M.ModifiedByName, M.ModifiedIP
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T ON T.ID = M.MiscTypeId
                        WHERE T.MiscTypeCode = @MiscTypeCode AND M.Code = @MiscCode
                        AND M.IsDeleted = 0 AND M.IsActive = 1
                        ORDER BY M.ID DESC";

                    var parameters = new
                    {
                        MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceStatusUpdate.Code
                    };

                    var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
                    return result.ToList();
                }

                public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceOpenstatusAsync()
                {
                    const string query = @"
                        SELECT M.Id, MiscTypeId, Code, M.Description, SortOrder, M.IsActive,
                            M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP,
                            M.ModifiedBy, M.ModifiedDate, M.ModifiedByName, M.ModifiedIP
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T ON T.ID = M.MiscTypeId
                        WHERE T.MiscTypeCode = @MiscTypeCode AND M.Code = @MiscCode
                        AND M.IsDeleted = 0 AND M.IsActive = 1
                        ORDER BY M.ID DESC";

                    var parameters = new
                    {
                        MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode,
                        MiscCode = MiscEnumEntity.StatusOpen.Code
                    };

                    var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
                    return result.ToList();
                }

                public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceRequestTypeAsync()
                {
                    const string query = @"
                        SELECT M.Id, MiscTypeId, Code, M.Description, SortOrder, M.IsActive,
                            M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP,
                            M.ModifiedBy, M.ModifiedDate, M.ModifiedByName, M.ModifiedIP
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T ON T.ID = M.MiscTypeId
                        WHERE  M.Code = @MiscCode AND T.MiscTypeCode = @MiscTypeCode
                        AND M.IsDeleted = 0 AND M.IsActive = 1
                        ORDER BY M.ID DESC";

                    var parameters = new
                    {
                        MiscTypeCode = MiscEnumEntity.MaintenanceRequestType.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeInternal.Code
                    };

                    var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
                    return result.ToList();
                }

                 public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceExternalRequestTypeAsync()
                {
                    const string query = @"
                        SELECT M.Id, MiscTypeId, Code, M.Description, SortOrder, M.IsActive,
                            M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP,
                            M.ModifiedBy, M.ModifiedDate, M.ModifiedByName, M.ModifiedIP
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T ON T.ID = M.MiscTypeId
                        WHERE  M.Code = @MiscCode AND T.MiscTypeCode = @MiscTypeCode
                        AND M.IsDeleted = 0 AND M.IsActive = 1
                        ORDER BY M.ID DESC";

                    var parameters = new
                    {
                        MiscTypeCode = MiscEnumEntity.MaintenanceRequestType.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceRequestTypeExternal.Code
                    };

                    var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
                    return result.ToList();
                }
                
                public async Task<bool> GetWOclosedAsync(int Id)
                {                      
                                var query = @"
                    SELECT COUNT(1)
                    FROM Maintenance.WorkOrder WO
                    INNER JOIN Maintenance.MiscMaster M ON M.Id = WO.StatusId
                    INNER JOIN Maintenance.MiscTypeMaster T ON T.Id = M.MiscTypeId
                    WHERE WO.RequestId = @RequestId
                    AND M.Code = @MiscCode
                    AND T.MiscTypeCode = @MiscTypeCode";
                   var parameters = new
                    {
                        RequestId = Id,
                        MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceStatusUpdate.Code 
                    };
                    
                     var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                        return count > 0;
                } 


                 public async Task<bool> GetWOclosedOrInProgressAsync(int id)
                {                      
                                var query = @"
                    SELECT COUNT(1)
                    FROM Maintenance.WorkOrder WO
                    INNER JOIN Maintenance.MiscMaster M ON M.Id = WO.StatusId
                    INNER JOIN Maintenance.MiscTypeMaster T ON T.Id = M.MiscTypeId
                    WHERE WO.RequestId = @RequestId And M.Code <> @MiscCode                   
                    AND T.MiscTypeCode = @MiscTypeCode";
                   var parameters = new
                    {
                        RequestId = id,
                        MiscTypeCode = MiscEnumEntity.WOStatus.MiscCode,
                        MiscCode= MiscEnumEntity.StatusOpen.Code ,
                       // MiscCodeInProgress = MiscEnumEntity.GetStatusId.Status


                    };
                    
                     var count = await _dbConnection.ExecuteScalarAsync<int>(query, parameters);
                        return count > 0;
                } 

                  public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceStatusDescAsync()
                    {
                    const string query = @"
                        SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
                        WHERE (MiscTypeCode = @MiscTypeCode) 
                        AND  M.IsDeleted=0 and M.IsActive=1
                        ORDER BY SortOrder DESC";    
                        var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceRequestType.MiscCode };        
                        var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
                        return result.ToList();
                    } 

                    public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceServiceDescAsync()
                    {
                    const string query = @"
                        SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
                        WHERE (MiscTypeCode = @MiscTypeCode) 
                        AND  M.IsDeleted=0 and M.IsActive=1
                        ORDER BY SortOrder DESC";    
                        var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceServiceType.MiscCode };        
                        var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
                        return result.ToList();
                    }    
                    public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceServiceLocationDescAsync()
                    {
                    const string query = @"
                        SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
                        WHERE (MiscTypeCode = @MiscTypeCode) 
                        AND  M.IsDeleted=0 and M.IsActive=1
                        ORDER BY SortOrder DESC";    
                        var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceServiceLocation.MiscCode };        
                        var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
                        return result.ToList();
                    }    

                    public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceSpareTypeDescAsync()
                    {
                    const string query = @"
                        SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
                        WHERE (MiscTypeCode = @MiscTypeCode) 
                        AND  M.IsDeleted=0 and M.IsActive=1
                        ORDER BY SortOrder DESC";    
                        var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceSpareType.MiscCode };        
                        var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
                        return result.ToList();
                    } 
                      
                    
                    public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenanceDispatchModeDescAsync()
                    {
                    const string query = @"
                        SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder            
                        FROM Maintenance.MiscMaster M
                        INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
                        WHERE (MiscTypeCode = @MiscTypeCode) 
                        AND  M.IsDeleted=0 and M.IsActive=1
                        ORDER BY SortOrder DESC";    
                        var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceDispatchMode.MiscCode };        
                        var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
                        return result.ToList();
                    } 

                    public async Task<List<RequestReportDto>> MaintenanceReportAsync(DateTimeOffset? requestFromDate, DateTimeOffset? requestToDate, int? RequestType, int? requestStatus )
                    {
                        var query = @"
                        SELECT 
                            a.Id AS RequestId,
                            a.UnitId,
                            a.CreatedDate AS RequestCreatedDate,
                            a.CreatedBy,
                            d.FirstName AS RequestCreatedName,
                            a.DepartmentId,
							G.DeptName as Department,
                            a.MachineId,
							E.MachineName,
                            a.MaintenanceTypeId,
							F.Code as MaintenanceType,
                            b.Id AS WorkOrderId,
                            b.StatusId,
							H.Code as RequestStatus,
                            a.ModifiedDate,
							A.OldVendorId,
							A.OldVendorName,
							A.ModeOfDispatchId,
                            L.Code as ModeOfDispatch,
							A.ExpectedDispatchDate,
							A.EstimatedSpareCost,
							A.EstimatedServiceCost,
							A.ServiceLocationId,
							I.Code AS ServiceLocation,
							A.ServiceTypeId,
							J.code AS ServiceType,
							A.SparesTypeId,
							K.Code  AS SparesType,
                            A.ModeOfDispatchId,
                            L.Code AS DispatchMode,
                            DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) AS RequestMinutesDifference,
                            RIGHT('0' + CAST(DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) / 60 AS VARCHAR), 2) + ':' +
                            RIGHT('0' + CAST(DATEDIFF(MINUTE, a.CreatedDate, a.ModifiedDate) % 60 AS VARCHAR), 2) AS DownTime,
                            RIGHT('0' + CAST(SUM(DATEDIFF(SECOND, c.StartTime, c.EndTime)) / 3600 AS VARCHAR), 2) + ':' +
                            RIGHT('0' + CAST((SUM(DATEDIFF(SECOND, c.StartTime, c.EndTime) % 3600) / 60) AS VARCHAR), 2) AS TimeTakenToRepair
                          FROM Maintenance.MaintenanceRequest a
                               LEFT JOIN Maintenance.WorkOrder b ON a.Id = b.RequestId
                                LEFT JOIN Maintenance.WorkOrderSchedule c ON c.WorkOrderId = b.Id
                                LEFT JOIN BANNARI.AppSecurity.Users d ON a.CreatedBy = d.UserId                        
                                LEFT JOIN  Maintenance.MachineMaster E  ON A.MachineId=E.Id 
                                LEFT JOIN  Maintenance.MiscMaster F  ON A.MaintenanceTypeId=F.Id 
                                LEFT JOIN  Bannari.AppData.Department G  ON A.DepartmentId=G.Id
                                LEFT JOIN  Maintenance.MiscMaster H  ON A.RequestStatusId=H.Id
                                LEFT JOIN  Maintenance.MiscMaster I  ON A.ServiceLocationId=I.Id
                                LEFT JOIN  Maintenance.MiscMaster J  ON A.ServiceTypeId=J.Id
                                LEFT JOIN  Maintenance.MiscMaster K  ON A.SparesTypeId=K.Id
                                LEFT JOIN  Maintenance.MiscMaster L  ON A.ModeOfDispatchId=L.Id
                            WHERE a.CreatedDate >= @RequestFromDate
                                AND a.CreatedDate <= @RequestToDate
                                AND (@RequestType IS NULL OR a.RequestTypeId = @RequestType)
                                AND (@RequestStatus IS NULL OR a.RequestStatusId = @RequestStatus)
                                AND (@RequestType IS NULL OR a.RequestTypeId = @RequestType)                      
                        GROUP BY 
                              a.Id, a.UnitId, a.CreatedDate, a.CreatedBy, a.DepartmentId,
                                a.MachineId, a.MaintenanceTypeId, b.Id, b.StatusId, a.ModifiedDate,
                                d.FirstName, G.DeptName, E.MachineName, F.Code, H.Code,
                                a.OldVendorId, a.OldVendorName, a.ModeOfDispatchId, a.ExpectedDispatchDate,
                                a.EstimatedSpareCost, a.EstimatedServiceCost,
                                a.ServiceLocationId, I.Code, a.ServiceTypeId, J.Code,
                                a.SparesTypeId, K.Code,L.Code
                        ORDER BY a.Id";

                        var parameters = new
                        {
                            RequestFromDate = requestFromDate?.Date,
                            RequestToDate = requestToDate?.Date.AddDays(1).AddTicks(-1), // To include full day
                            RequestType = RequestType,
                            RequestStatus = requestStatus
                        };
                        
                        var result = await _dbConnection.QueryAsync<RequestReportDto>(query, parameters);
                        return result.ToList();
                    }

                             
                         

    }
}