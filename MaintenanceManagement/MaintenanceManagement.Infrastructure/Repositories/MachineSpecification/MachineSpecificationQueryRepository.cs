using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMachineSpecification;
using Core.Application.MachineSpecification.Command;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MachineSpecification
{
    public class MachineSpecificationQueryRepository : IMachineSpecificationQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;

        public MachineSpecificationQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<List<MachineSpecificationDto>> GetByIdAsync(int Id)
        {
            var unitId = _ipAddressService.GetUnitId();

            const string query = @"
                SELECT 
                    MS.Id,
                    MS.SpecificationId,
                    MM.Code as SpecificationName,
                    MC.Id as MachineId,
                    MS.IsActive 
                FROM 
                    Maintenance.MachineSpecification MS
                INNER JOIN Maintenance.MachineMaster MC ON MS.MachineId = MC.Id
                INNER JOIN Maintenance.MiscMaster MM ON MM.Id = MS.SpecificationId 
                WHERE 
                    MS.IsDeleted = 0 
                    AND MC.Id = @Id
                    AND MC.UnitId = @UnitId";

            var result = await _dbConnection.QueryAsync<MachineSpecificationDto>(query, new { Id, unitId });
            return result.ToList();
        }


        public async Task<MachineSpecificationDto?> GetBySpecificationIdAsync(int Id)
        {
            const string query = @"
                                SELECT * 
                                FROM 
                                Maintenance.MachineSpecification 
                                where Id=@Id AND IsDeleted=0";

            var machineMaster = await _dbConnection.QueryFirstOrDefaultAsync<MachineSpecificationDto>(query, new { Id});
            return machineMaster;
        }
    }
}