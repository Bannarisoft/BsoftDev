using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;

namespace BSOFT.Infrastructure.Repositories.AdminSecuritySettings
{
    public class AdminSecuritySettingsQueryRepository  :IAdminSecuritySettingsQueryRepository
    {
        private readonly IDbConnection _dbConnection; 

        public  AdminSecuritySettingsQueryRepository(IDbConnection dbConnection)
    {
         _dbConnection = dbConnection;
    }

    public async Task<List<Core.Domain.Entities.AdminSecuritySettings>> GetAllAdminSecuritySettingsAsync()
    {
        Console.WriteLine("Hello Handler");
        const string query = @"SELECT  * FROM AppSecurity.AdminSecuritySettings";
            return (await _dbConnection.QueryAsync<Core.Domain.Entities.AdminSecuritySettings>(query)).ToList();
        
    }

    }
}