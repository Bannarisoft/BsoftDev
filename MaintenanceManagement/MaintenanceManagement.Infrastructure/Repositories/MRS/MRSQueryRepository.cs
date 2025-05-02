using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMRS;
using Core.Application.MRS.Queries;
using Core.Application.MRS.Queries.GetCategory;
using Core.Application.MRS.Queries.GetDepartment;
using Core.Application.MRS.Queries.GetSubCostCenter;
using Core.Application.MRS.Queries.GetSubDepartment;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MRS
{
    public class MRSQueryRepository : IMRSQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public MRSQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<MCategoryDto>> GetCategory(string OldUnitcode)
        {
               OldUnitcode ??= string.Empty;

            var parameters = new 
            {
                OldUnitcode
            };

            var departments = await _dbConnection.QueryAsync<MCategoryDto>(
                "GetCategoryByOldUnitcode", // Stored Procedure Name
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return departments.ToList();
        }

        public async Task<List<MDepartmentDto>> GetMDepartment(string OldUnitcode)
        {
            OldUnitcode ??= string.Empty;

            var parameters = new 
            {
                OldUnitcode
            };

            var departments = await _dbConnection.QueryAsync<MDepartmentDto>(
                "GetDepartmentsByOldUnitcode", // Stored Procedure Name
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return departments.ToList();
        }

        
        public async Task<List<MSubCostCenterDto>> GetSubCostCenter(string OldUnitcode)
        {
            OldUnitcode ??= string.Empty;

            var parameters = new 
            {
                OldUnitcode
            };

            var departments = await _dbConnection.QueryAsync<MSubCostCenterDto>(
                "GetSubCostCentersByOldUnitcode", // Stored Procedure Name
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return departments.ToList();
        }

        public async Task<List<MSubDepartment>> GetSubDepartment(string OldUnitcode)
        {
             OldUnitcode ??= string.Empty;

            var parameters = new 
            {
                OldUnitcode
            };

            var departments = await _dbConnection.QueryAsync<MSubDepartment>(
                "GetSubDepartmentByOldUnitcode", // Stored Procedure Name
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return departments.ToList();
        }
    }
}