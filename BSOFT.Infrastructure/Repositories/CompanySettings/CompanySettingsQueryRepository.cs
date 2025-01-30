using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICompanySettings;
using Dapper;

namespace BSOFT.Infrastructure.Repositories.CompanySettings
{
    public class CompanySettingsQueryRepository : ICompanyQuerySettings
    {
        private readonly IDbConnection _dbConnection;

        public CompanySettingsQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Core.Domain.Entities.CompanySettings> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM AppData.CompanySetting WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.CompanySettings>(query, new { id });
        }
    }
}