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

                public async Task<(List<Core.Domain.Entities.MaintenanceRequest>, int)> GetAllMaintenanceRequestAsync(int PageNumber, int PageSize, string? SearchTerm)
                {
                                        var query = $$"""
                        DECLARE @TotalCount INT;

                        SELECT @TotalCount = COUNT(*)
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        WHERE A.IsDeleted = 0
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search  OR E.MachineName LIKE @Search)")}};
                                            
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
                            A.IsActive,
                            A.IsDeleted,
                            A.CreatedBy,
                            A.CreatedDate,
                            A.CreatedByName,
                            A.CreatedIP,
                            A.ModifiedBy,
                            A.ModifiedDate,
                            A.ModifiedByName,
                            A.ModifiedIP,
                            B.Id,
                            B.Code,
                            C.Id,
                            C.Code,
                            E.Id,
                            E.MachineName
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id                        
                        WHERE A.IsDeleted = 0
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(A.Id AS NVARCHAR) LIKE @Search OR A.Remarks LIKE @Search OR B.Code LIKE @Search OR C.Code LIKE @Search  OR E.MachineName LIKE @Search)")}}
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

               

                var maintenanceReqList = multi.Read<Core.Domain.Entities.MaintenanceRequest, Core.Domain.Entities.MiscMaster, Core.Domain.Entities.MiscMaster, Core.Domain.Entities.MachineMaster,Core.Domain.Entities.MaintenanceRequest>(
                (maintenanceReq,requestType ,maintenanceType, machineMaster) =>
                {
                    maintenanceReq.MiscRequestType = new Core.Domain.Entities.MiscMaster
                    {
                        Id = requestType.Id,
                        Code = requestType.Code
                    };

                    maintenanceReq.MiscMaintenanceType = new Core.Domain.Entities.MiscMaster
                    {
                        Id = maintenanceType.Id,
                        Code = maintenanceType.Code
                    };
                    maintenanceReq.Machine = new Core.Domain.Entities.MachineMaster
                    {
                        Id = machineMaster.Id,
                        MachineName = machineMaster.MachineName
                    };

                    



                    return maintenanceReq;
                },
                splitOn: "Id,Id,Id"
            ).ToList();

            var totalCount = multi.ReadFirst<int>();

            
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
                            A.IsActive,
                            A.IsDeleted,
                            A.CreatedBy,
                            A.CreatedDate,
                            A.CreatedByName,
                            A.CreatedIP,
                            A.ModifiedBy,
                            A.ModifiedDate,
                            A.ModifiedByName,
                            A.ModifiedIP,
                            B.Id,
                            B.Code,
                            C.Id,
                            C.Code,
                            E.Id,
                            E.MachineName
                        FROM Maintenance.MaintenanceRequest A
                        INNER JOIN Maintenance.MiscMaster B ON A.RequestTypeId = B.Id
                        INNER JOIN Maintenance.MiscMaster C ON A.MaintenanceTypeId = C.Id                        
                        INNER JOIN Maintenance.MachineMaster E ON A.MachineId = E.Id
                        WHERE A.Id = @Id AND A.IsDeleted = 0";

                    var maintenanceRequest = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MaintenanceRequest>(query, new { Id });
                    
                        var result = await _dbConnection.QueryAsync<
                    Core.Domain.Entities.MaintenanceRequest,
                    Core.Domain.Entities.MiscMaster,
                    Core.Domain.Entities.MiscMaster,
                    Core.Domain.Entities.MachineMaster,
                    Core.Domain.Entities.MaintenanceRequest>(
                    query,
                    (maintenanceReq, requestType, maintenanceType, machineMaster) =>
                    {
                        maintenanceReq.MiscRequestType = requestType;
                        maintenanceReq.MiscMaintenanceType = maintenanceType;
                        maintenanceReq.Machine = machineMaster;
                        return maintenanceReq;
                    },
                    new { Id = Id },
                    splitOn: "Id,Id,Id");

                return result.FirstOrDefault();
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

                  

    }
}