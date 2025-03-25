using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IMiscMaster;
using Dapper;

namespace MaintenanceManagement.Infrastructure.Repositories.MiscMaster
{
    public class MiscMasterQueryRepository : IMiscMasterQueryRepository
    {
    private readonly IDbConnection _dbConnection;

     public MiscMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;

        }

        public async Task<(List<Core.Domain.Entities.MiscMaster>,int)> GetAllMiscMasterAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
                var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM [Maintenance].[MiscMaster] M
                WHERE M.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (M.Code LIKE @Search)")}}; 

                SELECT M.Id, M.MiscTypeId, M.Code, M.Description, M.SortOrder, M.IsActive, M.IsDeleted, 
                    M.CreatedBy, M.CreatedDate, M.CreatedByName, M.CreatedIP, M.ModifiedBy, M.ModifiedDate, 
                    M.ModifiedByName, M.ModifiedIP
                FROM Maintenance.MiscMaster M
                WHERE M.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (M.Code LIKE @Search)")}}
                ORDER BY M.Id DESC 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
                """;

            var parameters = new
            {
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var result = await _dbConnection.QueryMultipleAsync(query, parameters);
            
            // Read the data for MiscMaster and convert to list
            var miscMasterList = (await result.ReadAsync<Core.Domain.Entities.MiscMaster>()).ToList();
            
            // Read the total count
            int totalCount = await result.ReadFirstAsync<int>();

            return (miscMasterList, totalCount);
                
            }

            
            public async Task<Core.Domain.Entities.MiscMaster> GetByIdAsync(int id)
        {            
           const string query = @" SELECT Id,MiscTypeId,Code,Description,SortOrder,IsActive  FROM Maintenance.MiscMaster          
             WHERE Id = @id AND IsDeleted = 0 ";                          
            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MiscMaster>(query, new { id });
        } 

        public async Task<List<Core.Domain.Entities.MiscMaster>>  GetMiscMaster(string searchPattern)
        {
            

            const string query = @"SELECT Id,Code   FROM Maintenance.MiscMaster
                WHERE IsDeleted = 0 AND Code LIKE @SearchPattern ";
                
            
            var parameters = new 
              { 
                  SearchPattern = $"%{searchPattern ?? string.Empty}%", 
             
              };

            var miscmaster = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
            return miscmaster.ToList();
        }

        public async Task<Core.Domain.Entities.MiscMaster?> GetByMiscMasterCodeAsync(string name, int? id = null)
        {
              var query = """
                 SELECT * FROM Maintenance.MiscMaster
                 WHERE Code = @Name AND IsDeleted = 0
                 """;

             var parameters = new DynamicParameters(new { Name = name });

             if (id is not null)
             {
                 query += " AND Id != @Id";
                 parameters.Add("Id", id);
             }

            return await _dbConnection.QueryFirstOrDefaultAsync<Core.Domain.Entities.MiscMaster>(query, parameters);
        } 

                    public async Task<int> GetMaxSortOrderAsync()
            {
                var query = "SELECT ISNULL(MAX(SortOrder), 0) FROM Maintenance.MiscMaster WHERE IsDeleted = 0";
                return await _dbConnection.QueryFirstOrDefaultAsync<int>(query);
            }
                    


    }   

    
} 