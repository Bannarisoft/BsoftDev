using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICompanySettings;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.CompanySettings
{
    public class CompanySettingsQueryRepository : ICompanyQuerySettings
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;

        public CompanySettingsQueryRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }
        public async Task<Core.Domain.Entities.CompanySettings> GetAsync()
        {
              var companyId = _ipAddressService.GetCompanyId();
       
            const string query = "SELECT * FROM AppData.CompanySetting where CompanyId = @CompanyId";
            
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.CompanySettings>(query, new { CompanyId = companyId });
        }
         public async Task<bool> AlreadyExistsAsync(int CompanyId, int? id = null)
          {
              var sql = "SELECT COUNT(1) FROM [AppData].[CompanySetting] WHERE CompanyId = @CompanyId AND IsDeleted = 0";
              var parameters = new DynamicParameters(new { CompanyId = CompanyId });

               if (id is not null)
             {
                 sql += " AND Id != @Id";
                 parameters.Add("Id", id);
             }
                var count = await _dbConnection.ExecuteScalarAsync<int>(sql, parameters);
                return count > 0;
          }
        
    }
}