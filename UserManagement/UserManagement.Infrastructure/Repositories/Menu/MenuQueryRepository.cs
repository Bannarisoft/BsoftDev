using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMenu;
using Dapper;

namespace UserManagement.Infrastructure.Repositories.Menu
{
    public class MenuQueryRepository : IMenuQuery
    {
        private readonly IDbConnection _dbConnection;
        public MenuQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<Core.Domain.Entities.Menu>> GetChildMenus(List<int> parentId)
        {
            string parentIdList = string.Join(",", parentId);
            string query = $@"
              WITH RecursiveMenu AS (
                  SELECT Id, ModuleId, ParentId, MenuName, MenuUrl
                  FROM [AppData].[Menus]
                  WHERE ParentId IN ({parentIdList})
                  UNION ALL
                  SELECT m.Id, m.ModuleId, m.ParentId, m.MenuName, m.MenuUrl
                  FROM [AppData].[Menus] m
                  INNER JOIN RecursiveMenu rm ON m.ParentId = rm.Id
              )
              SELECT * FROM RecursiveMenu;";

               var childMenus = await _dbConnection.QueryAsync<Core.Domain.Entities.Menu>(query);
            return childMenus.ToList();
        }

        public async Task<List<Core.Domain.Entities.Menu>> GetParentMenus(List<int> moduleId)
        {
            string moduleIdList = string.Join(",", moduleId);
             string query = $@"
                                  SELECT Id, MenuName 
                                  FROM AppData.Menus 
                                  WHERE IsDeleted = 0 AND ModuleId IN ({moduleIdList}) AND ParentId = 0
                                  ";
                
            var parentMenus = await _dbConnection.QueryAsync<Core.Domain.Entities.Menu>(query);
            return parentMenus.ToList();
        }
    }
}