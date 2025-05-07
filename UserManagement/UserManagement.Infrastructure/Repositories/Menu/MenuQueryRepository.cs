using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IMenu;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.Menu
{
    public class MenuQueryRepository : IMenuQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public MenuQueryRepository(IDbConnection dbConnection,IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = ipAddressService;
        }

        public async Task<List<Core.Domain.Entities.Menu>> GetChildMenus(List<int> parentId)
        {
            var companyId = _ipAddressService.GetCompanyId();
            string parentIdList = string.Join(",", parentId);
            string query = $@"
              WITH RecursiveMenu AS (
                  SELECT Id, ModuleId, ParentId, MenuName, MenuUrl
                  FROM [AppData].[Menus]
                  WHERE ParentId IN ({parentIdList}) AND CompanyId=@companyId
                  UNION ALL
                  SELECT m.Id, m.ModuleId, m.ParentId, m.MenuName, m.MenuUrl
                  FROM [AppData].[Menus] m
                  INNER JOIN RecursiveMenu rm ON m.ParentId = rm.Id AND m.CompanyId=@companyId
              )
              SELECT * FROM RecursiveMenu;";

               var childMenus = await _dbConnection.QueryAsync<Core.Domain.Entities.Menu>(query,companyId);
            return childMenus.ToList();
        }

        public async Task<List<Core.Domain.Entities.Menu>> GetParentMenus(List<int> moduleId)
        {
            var companyId = _ipAddressService.GetCompanyId();
            string moduleIdList = string.Join(",", moduleId);
             string query = $@"
                                  SELECT Id, MenuName 
                                  FROM AppData.Menus 
                                  WHERE IsDeleted = 0 AND ModuleId IN ({moduleIdList}) AND ParentId = 0 AND CompanyId=@companyId
                                  ";
                
            var parentMenus = await _dbConnection.QueryAsync<Core.Domain.Entities.Menu>(query,companyId);
            return parentMenus.ToList();
        }
         public async Task<bool> FKColumnExistValidation(int Id)
          {
            var companyId = _ipAddressService.GetCompanyId();
              var sql = "SELECT COUNT(1) FROM AppData.Menus WHERE Id = @Id AND IsDeleted = 0 AND IsActive = 1 AND CompanyId=@CompanyId ";
                var count = await _dbConnection.ExecuteScalarAsync<int>(sql, new { Id = Id,CompanyId=companyId });
                return count > 0;
          }
    }
}