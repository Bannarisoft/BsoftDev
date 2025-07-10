using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.Power.IGeneratorConsumption;
using Core.Application.Power.GeneratorConsumption.Queries.GetClosingEnergyReaderValueById;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.Power.GeneratorConsumption
{
    public class GeneratorConsumptionQueryRepository : IGeneratorConsumptionQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
          public GeneratorConsumptionQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<GetClosingEnergyReaderValueDto> GetOpeningReaderValueById(int generatorId)
        {
            var UnitId = _ipAddressService.GetUnitId();
            const string query = @"
                SELECT 
                    f.Id AS GeneratorId,
                    f.MachineCode as GeneratorCode,
                    f.MachineName as GeneratorName,
                    ISNULL(
                        (
                            SELECT TOP 1 pc.ClosingEnergyReading
                            FROM Maintenance.GeneratorConsumption pc
                            WHERE pc.GeneratorId = f.Id AND pc.IsDeleted = 0 And pc.UnitId = @UnitId
                            ORDER BY pc.CreatedDate DESC
                        ),
                        f.ProductionCapacity
                    ) AS OpeningEnergyReading
                FROM Maintenance.MachineMaster f
                WHERE f.Id = @GeneratorId AND f.IsDeleted = 0 AND f.UnitId = @UnitId;";

                var result = await _dbConnection.QueryFirstOrDefaultAsync<GetClosingEnergyReaderValueDto>(query, new { GeneratorId = generatorId, UnitId = UnitId });

            if (result == null)
                throw new Exception($"Generator with ID {generatorId} not found.");

            return result;
        }
    }
}