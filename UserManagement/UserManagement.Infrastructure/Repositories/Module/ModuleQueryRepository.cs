using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IModule;
using System.Data;
using Dapper;


namespace UserManagement.Infrastructure.Repositories.Module
{
    public class ModuleQueryRepository : IModuleQueryRepository
    {
    private readonly IDbConnection _dbConnection;


    public ModuleQueryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;

    }
     public async Task<Modules> GetModuleByIdAsync(int id)
    {
              var sql = @"
                SELECT 
                    m.Id, m.ModuleName,mn.Id AS MenuId, mn.MenuName, mn.ModuleId 
                FROM [AppData].[Modules] m
                LEFT JOIN [AppData].[Menus] mn ON m.Id = mn.ModuleId
                WHERE m.Id = @ModuleId AND m.IsDeleted = 0
                ORDER BY m.Id";

            var moduleDictionary = new Dictionary<int, Modules>();

            var result = await _dbConnection.QueryAsync<Modules, Menu, Modules>(
                sql,
                (module, menu) =>
                {
                    if (!moduleDictionary.TryGetValue(module.Id, out var currentModule))
                    {
                        currentModule = module;
                        currentModule.Menus = new List<Menu>(); // Initialize list
                        moduleDictionary.Add(module.Id, currentModule);
                    }

                    if (menu != null)
                        currentModule.Menus.Add(menu);

                    return currentModule;
                },
                new { ModuleId = id },
                splitOn: "MenuId"
            );

            return moduleDictionary.Values.FirstOrDefault();
        // return await _applicationDbContext.Modules.Include(m => m.Menus).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Modules>> GetAllModulesAsync()
    {
         var sql = @"
                SELECT 
                    m.Id, m.ModuleName,mn.Id AS MenuId, mn.MenuName, mn.ModuleId 
                FROM [AppData].[Modules] m
                LEFT JOIN [AppData].[Menus] mn ON m.Id = mn.ModuleId
                WHERE m.IsDeleted = 0
                ORDER BY m.Id";

            var moduleDictionary = new Dictionary<int, Modules>();

            var result = await _dbConnection.QueryAsync<Modules, Menu, Modules>(
                sql,
                (module, menu) =>
                {
                    if (!moduleDictionary.TryGetValue(module.Id, out var currentModule))
                    {
                        currentModule = module;
                        currentModule.Menus = new List<Menu>();
                        moduleDictionary.Add(module.Id, currentModule);
                    }

                    if (menu != null)
                        currentModule.Menus.Add(menu);

                    return currentModule;
                },
                splitOn: "MenuId"
            );

            return moduleDictionary.Values.ToList();
        // return await _applicationDbContext.Modules.Include(m => m.Menus).ToListAsync();
    }  
    }
}