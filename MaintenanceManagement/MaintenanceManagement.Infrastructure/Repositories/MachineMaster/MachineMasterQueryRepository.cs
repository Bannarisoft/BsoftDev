using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineDepartmentbyId;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Application.MachineMaster.Queries.GetMachineNoDepartmentbyId;
using Core.Domain.Common;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MachineMaster
{
    public class MachineMasterQueryRepository : IMachineMasterQueryRepository
    {
         private readonly IDbConnection _dbConnection; 
         private readonly IIPAddressService _ipAddressService;  
         public MachineMasterQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }
        public async Task<(List<MachineMasterDto>, int)> GetAllMachineAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
           var query = $$"""
                        DECLARE @TotalCount INT;

                        -- Count total records
                        SELECT @TotalCount = COUNT(*) 
                        FROM Maintenance.MachineMaster mm
                        LEFT JOIN Maintenance.MachineGroup mg ON mm.MachineGroupId = mg.Id
                        WHERE mm.IsDeleted = 0 AND mm.UnitId = @UnitId
                        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (mm.MachineCode LIKE @Search OR mm.MachineName LIKE @Search OR mg.GroupName LIKE @Search)")}};

                        -- Fetch paged records with MachineGroupName
                        SELECT 
                            mm.Id, 
                            mm.MachineCode,
                            mm.MachineName,
                            mm.MachineGroupId,
                            mg.GroupName AS MachineGroupName, -- <- correct mapping
                            mm.UnitId,
                            mm.ProductionCapacity,
                            mm.UomId,
                            mm.ShiftMasterId,
                            mm.CostCenterId,
                            mm.WorkCenterId,
                            mm.InstallationDate,
                            mm.AssetId,
                            mm.[LineNo],
                            mm.IsActive
                        FROM Maintenance.MachineMaster mm
                        LEFT JOIN Maintenance.MachineGroup mg ON mm.MachineGroupId = mg.Id
                        WHERE 
                            mm.IsDeleted = 0 AND mm.UnitId = @UnitId
                            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (mm.MachineCode LIKE @Search OR mm.MachineName LIKE @Search OR mg.GroupName LIKE @Search)")}}
                        ORDER BY mm.Id DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                        -- Return total count
                        SELECT @TotalCount AS TotalCount;
                        """;
            
             var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize,
                           UnitId
                       };

             var maintenanceCategory = await _dbConnection.QueryMultipleAsync(query, parameters);
             var maintenanceCategorylist = (await maintenanceCategory.ReadAsync<MachineMasterDto>()).ToList();
             int totalCount = (await maintenanceCategory.ReadFirstAsync<int>());
             return (maintenanceCategorylist, totalCount);
        }

        public async Task<MachineMasterDto?> GetByIdAsync(int Id)
        {
             var unitId = _ipAddressService.GetUnitId();
             const string query = @"
                                SELECT 
                                mm.*, 
                                mg.GroupName AS MachineGroupName
                            FROM 
                                Maintenance.MachineMaster mm
                            LEFT JOIN 
                                Maintenance.MachineGroup mg ON mm.MachineGroupId = mg.Id
                            WHERE 
                                mm.Id = @Id 
                                AND mm.IsDeleted = 0 
                                AND mm.UnitId = @UnitId";

                    var machineMaster = await _dbConnection.QueryFirstOrDefaultAsync<MachineMasterDto>(query, new { Id,unitId });
                    return machineMaster;
        }
        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineAsync(string searchPattern)
        {
            var unitId = _ipAddressService.GetUnitId();
            searchPattern = searchPattern ?? string.Empty; // Prevent null issues 
            const string query = @"SELECT 
                M.Id, M.MachineName, M.MachineCode, M.MachineGroupId,
                MG.Id AS MachineGroupId, 
                MG.DepartmentId
                FROM Maintenance.MachineMaster M
                INNER JOIN Maintenance.MachineGroup MG ON M.MachineGroupId = MG.Id 
                WHERE M.IsDeleted = 0 
                AND M.UnitId = @UnitId 
                AND (M.MachineName LIKE @SearchPattern OR M.MachineCode LIKE @SearchPattern)";

           var lookup = new Dictionary<int, Core.Domain.Entities.MachineMaster>();
                        var machines = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster, Core.Domain.Entities.MachineGroup, Core.Domain.Entities.MachineMaster>(
                    query,
                    (machine, group) =>
                    {
                        if (!lookup.TryGetValue(machine.Id, out var machineEntry))
                        {
                            machineEntry = machine;
                            machineEntry.MachineGroup = group;
                            lookup[machine.Id] = machineEntry;
                        }

                        return machineEntry;
                    },
                    new { UnitId = unitId, SearchPattern = $"%{searchPattern}%" },
                    splitOn: "MachineGroupId" 
                );

             foreach (var machine in machines)
            {
                var departmentId = machine.MachineGroup.DepartmentId;
               
            }

            return lookup.Values.ToList();  

        }

        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupAsync(int MachineGroupId)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @"
                    SELECT Id 
                    FROM Maintenance.MachineMaster 
                    WHERE MachineGroupId = @MachineGroupId AND IsDeleted = 0 AND UnitId = @UnitId";

            var machineMaster = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, new { MachineGroupId, UnitId });
            return machineMaster.ToList();
        }
        // public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroup(int MachineGroupId)
        // {
        //     searchPattern = searchPattern ?? string.Empty; // Prevent null issues

        //     const string query = @"
        //      SELECT M.Id, M.MachineName 
        //     FROM Maintenance.MachineMaster M
        //     LEFT JOIN [Maintenance].[WorkOrder] WO on WO.MachineId=M.Id
        //     WHERE IsDeleted = 0 
        //     AND MachineName LIKE @SearchPattern";  
        //     var parameters = new 
        //     { 
        //     SearchPattern = $"%{searchPattern}%" 
        //     };

        //     var machineMasters = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, parameters);
        //     return machineMasters.ToList();
        // }
        public async Task<List<Core.Domain.Entities.MachineMaster>> GetMachineByGroupSagaAsync(int MachineGroupId,int UnitId)
        {
            
             const string query = @"
                    SELECT Id 
                    FROM Maintenance.MachineMaster 
                    WHERE MachineGroupId = @MachineGroupId AND IsDeleted = 0 AND UnitId = @UnitId";

                    var machineMaster = await _dbConnection.QueryAsync<Core.Domain.Entities.MachineMaster>(query, new { MachineGroupId, UnitId });
                    return machineMaster.ToList();
        }

        public async Task<MachineDepartmentGroupDto?> GetMachineDepartment(int MachineGroupId)
        {
           
             const string query = @"
                    select A.GroupName,B.DeptName as DepartmentName,C.DepartmentGroupName from Maintenance.MachineGroup A 
			        INNER JOIN Bannari.AppData.Department B On A.DepartmentId=B.Id 
			        INNER JOIN Bannari.AppData.DepartmentGroup C on B.DepartmentGroupId=C.Id WHERE A.Id=@MachineGroupId and B.IsDeleted=0 and C.IsDeleted=0";

                    var machineMaster = await _dbConnection.QueryFirstOrDefaultAsync<MachineDepartmentGroupDto>(query, new {MachineGroupId});
                    return machineMaster;
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetMachineLineNoAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM Maintenance.MiscMaster M
            INNER JOIN Maintenance.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.MachineLineNo.Code };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<List<GetMachineNoDepartmentbyIdDto>> GetMachineNoDepartmentAsync(int DepartmentId)
        {
           /*  const string query = @"
                SELECT A.Id, A.MachineCode, A.MachineName
                FROM Maintenance.MachineMaster A
                INNER JOIN Maintenance.MachineGroup B ON A.MachineGroupId = B.Id
                WHERE B.DepartmentId = @DepartmentId"; */
                var UnitId = _ipAddressService.GetUnitId();
                var CompanyId = _ipAddressService.GetCompanyId();
            const string query = @"
                select distinct MM.Id,MachineCode,MachineName 
                from Maintenance.WorkOrder WO with(nolock)              
                LEFT JOIN [Maintenance].[MaintenanceRequest]  MR with(nolock) on MR.ID=WO.RequestId                        
                LEFT JOIN [Maintenance].[PreventiveSchedulerDetail]  PS with(nolock) on PS.ID=WO.PreventiveScheduleId                        
                LEFT JOIN [Maintenance].[PreventiveSchedulerHeader] PH with(nolock) on PH.Id=PS.PreventiveSchedulerHeaderId  
                inner join Maintenance.MachineMaster MM on MM.Id= case when isnull(requestid,0)<>0 then MR.MachineId else PS.MachineId end
                INNER JOIN Maintenance.MiscMaster T on T.ID=WO.StatusId and T.Code not in('Closed','Cancelled')
                where WO.CompanyId=@CompanyId and WO.UnitId=@UnitId  and (case when isnull(requestid,0)<>0 then MR.MaintenanceDepartmentId else PH.DepartmentId end) = @DepartmentId   ";

            var result = await _dbConnection.QueryAsync<GetMachineNoDepartmentbyIdDto>(query, new {CompanyId=CompanyId,UnitId=UnitId ,DepartmentId = DepartmentId });

            return result.ToList();
        }
    }
}