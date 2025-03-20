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
           public async Task<Core.Domain.Entities.CompanySettings> BeforeLoginGetUserCompanySettings(string Username)
          {
              var sql = @"Declare @CompanyId int 
              SET @CompanyId = (SELECT TOP 1 UC.CompanyId FROM [AppSecurity].[Users] U
              INNER JOIN [AppSecurity].[UserCompany] UC ON UC.UserId = U.UserId  WHERE U.Username = @Username AND U.IsDeleted = 0 AND UC.IsActive = 1)

              SELECT * FROM [AppData].[CompanySetting] WHERE CompanyId = @CompanyId AND IsDeleted = 0";
             
                var companySettings = await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.CompanySettings>(sql, new { Username = Username });
                return companySettings;
          }
            public async Task<bool> BeforeLoginNotFoundValidation(string Username)
          {
            
                 var sql = @"Declare @EntityId int,@GroupCode nvarchar(50),@CompanyId int  

                 SET @GroupCode = (SELECT TOP 1 UG.GroupCode FROM [AppSecurity].[Users] U
                 INNER JOIN [AppSecurity].[UserGroup] UG ON UG.Id = U.UserGroupId
                 WHERE U.Username = @Username AND U.IsDeleted = 0 )

               IF @GroupCode = 'ENT_ADM_USR' || @GroupCode = 'ENT_ADM'
               BEGIN
                 SET @EntityId = (SELECT TOP 1 U.EntityId FROM [AppSecurity].[Users] U
                    WHERE U.Username = @Username AND U.IsDeleted = 0 )

                 SELECT COUNT(1)  FROM [AppSecurity].[AdminSecuritySettings] WHERE EntityId = @EntityId AND IsDeleted = 0
               END
               ELSE IF @GroupCode = 'COMP_ADM_USR' || @GroupCode = 'COMP_ADM'
               BEGIN

                SET @CompanyId = (SELECT TOP 1 UC.CompanyId FROM [AppSecurity].[Users] U
                INNER JOIN [AppSecurity].[UserCompany] UC ON UC.UserId = U.UserId  WHERE U.Username = @Username AND U.IsDeleted = 0 AND UC.IsActive = 1)

                SELECT COUNT(1)  FROM [AppData].[CompanySetting] WHERE CompanyId = @CompanyId AND IsDeleted = 0   
               END
               ";
                
            
            
             
            var companySettings = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { Username = Username });
            return companySettings > 0;
          }
        
    }
}