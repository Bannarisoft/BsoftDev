using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMachineGroup;
using Core.Application.Common.Interfaces.IMaintenanceRequest;
using Core.Application.MaintenanceRequest.Queries.GetExistingVendorDetails;
using Core.Application.MaintenanceRequest.Queries.GetMaintenanceRequest;
using Core.Domain.Common;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MaintenanceRequest
{
    public class MaintenanceRequestQueryRepository : IMaintenanceRequestQueryRepository
    {
                private readonly IDbConnection _dbConnection; 

                public MaintenanceRequestQueryRepository(IDbConnection dbConnection)
                {
                    _dbConnection = dbConnection;
                }

                public async Task<(IEnumerable<dynamic> MaintenanceRequestList, int)> GetAllMaintenanceRequestAsync(int PageNumber, int PageSize, string? SearchTerm)
                {
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
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search  OR F.Code LIKE @Search OR G.Code LIKE @Search OR E.MachineName LIKE @Search OR H.Code LIKE @Search OR I.Code LIKE @Search OR J.Code LIKE @Search ) ")}};
                                            
                        SELECT 
                            A.Id,
                            A.DepartmentId,
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,
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

                        WHERE A.IsDeleted = 0 AND A.RequestStatusId <>33
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search OR F.Code LIKE @Search or G.Code LIKE @Search OR H.Code LIKE @Search  OR E.MachineName LIKE @Search OR I.Code LIKE @Search  or J.Code LIKE @Search) ")}}
                        ORDER BY A.Id DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        SELECT @TotalCount AS TotalCount;
                        """;
                    var parameters = new
                    {
                        Search = $"%{SearchTerm}%",
                        Offset = (PageNumber - 1) * PageSize,
                        PageSize
                    };

                         using var multi = await _dbConnection.QueryMultipleAsync(query, parameters);
                       var maintenanceReqList = await multi.ReadAsync<dynamic>();
                       var totalCount = await multi.ReadFirstAsync<int>();
           

                    return (maintenanceReqList, totalCount);
                }


                public async Task<Core.Domain.Entities.MaintenanceRequest?> GetByIdAsync(int Id)
                {
                    const string query = @"
                        SELECT 
                            A.Id,
                            A.RequestTypeId,                           
                            A.MaintenanceTypeId,                         
                            A.MachineId,
                            A.DepartmentId,                            
                            A.SourceId,
                            A.VendorId,
                            A.OldVendorId,
                            A.Remarks,
                            A.IsActive
                        FROM Maintenance.MaintenanceRequest A
                        WHERE A.Id = @Id AND A.IsDeleted = 0";
                     return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MaintenanceRequest>(query, new { Id });
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

        //  public async Task<List<Core.Domain.Entities.MiscMaster>> GetMaintenancestatusAsync()
        // {
        //     const string query = @"
        //             SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder, M.IsActive
        //             ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
        //             FROM Maintenance.MiscMaster M
        //             INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
        //             WHERE (MiscTypeCode = @MiscTypeCode)  AND M.Code = @MiscCode
        //             AND M.IsDeleted=0 and M.IsActive=1
        //             ORDER BY M.ID DESC";
        //             var parameters = new { MiscTypeCode = MiscEnumEntity.MaintenanceStatus.MiscCode ,MiscCode= MiscEnumEntity.MaintenanceStatusUpdate.Code };
        //             var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
        //             return result.ToList();
        // } 


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
                        MiscTypeCode = MiscEnumEntity.MaintenanceStatus.MiscCode,
                        MiscCode = MiscEnumEntity.MaintenanceStatusUpdate.Code
                    };

                    var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
                    return result.ToList();
                }



        

                  

    }
}