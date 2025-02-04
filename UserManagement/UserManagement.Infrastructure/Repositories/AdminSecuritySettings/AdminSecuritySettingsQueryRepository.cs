using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Data;
using Dapper;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using System.Security.Permissions;


namespace UserManagement.Infrastructure.Repositories.AdminSecuritySettings
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

    public async Task<Core.Domain.Entities.AdminSecuritySettings> GetAdminSecuritySettingsByIdAsync(int id)
    {
         // const string query = @"SELECT * FROM AdminSecuritySettings WHERE Id = @Id";
   // return await _dbConnection.QueryFirstOrDefaultAsync<AdminSecuritySettings>(query, new { Id = id });

        const string query = @"SELECT * FROM AppSecurity.AdminSecuritySettings WHERE Id = @Id ";
            var adminsettings = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AdminSecuritySettings>(query, new { id });
            
            if (adminsettings == null)
            {
                throw new KeyNotFoundException($"Admin Security Settings with ID {id} not found.");
            }

            return adminsettings;
    }



    }
}