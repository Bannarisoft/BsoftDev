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

    // public async Task<List<Core.Domain.Entities.AdminSecuritySettings>> GetAllAdminSecuritySettingsAsync()
    // {
        
    //     const string query = @"SELECT  * FROM AppSecurity.AdminSecuritySettings WHERE IsDeleted = 0 ORDER BY ID DESC";
    //         return (await _dbConnection.QueryAsync<Core.Domain.Entities.AdminSecuritySettings>(query)).ToList();
        
    // }
            public async Task<(List<Core.Domain.Entities.AdminSecuritySettings>, int)> GetAllAdminSecuritySettingsAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM AppSecurity.AdminSecuritySettings
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Id LIKE @Search)")}};
                
                SELECT Id,
                    PasswordHistoryCount,
                    SessionTimeoutMinutes,
                    MaxFailedLoginAttempts,
                    AccountAutoUnlockMinutes,
                    PasswordExpiryDays,
                    PasswordExpiryAlertDays,
                    IsTwoFactorAuthenticationEnabled,
                    MaxConcurrentLogins,
                    IsForcePasswordChangeOnFirstLogin,
                    PasswordResetCodeExpiryMinutes,
                    IsCaptchaEnabledOnLogin,
                    IsActive,
                    CreatedBy,
                    CreatedAt,
                    CreatedByName,
                    CreatedIP,
                    ModifiedBy,
                    ModifiedAt,
                    ModifiedByName,
                    ModifiedIP,
                    IsDeleted
                FROM AppSecurity.AdminSecuritySettings
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Id LIKE @Search)")}}
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                
                SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var securitySettings = await _dbConnection.QueryMultipleAsync(query, parameters);
            var settingsList = (await securitySettings.ReadAsync<Core.Domain.Entities.AdminSecuritySettings>()).ToList();
            int totalCount = (await securitySettings.ReadFirstAsync<int>());

            return (settingsList, totalCount);
        }


        public async Task<Core.Domain.Entities.AdminSecuritySettings> GetAdminSecuritySettingsByIdAsync(int id)
        {
            
            const string query = @"SELECT * FROM AppSecurity.AdminSecuritySettings WHERE Id = @Id AND IsDeleted = 0 ORDER BY ID DESC";
                var adminsettings = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.AdminSecuritySettings>(query, new { id });
                
                if (adminsettings == null)
                {
                    throw new KeyNotFoundException($"Admin Security Settings with ID {id} not found.");
                }

                return adminsettings;
        }



    }
}