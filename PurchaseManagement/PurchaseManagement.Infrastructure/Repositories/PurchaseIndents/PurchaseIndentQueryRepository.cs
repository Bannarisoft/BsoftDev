using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using Dapper;

namespace PurchaseManagement.Infrastructure.Repositories.PurchaseIndents
{
    public class PurchaseIndentQueryRepository : IPurchaseIndentQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        public PurchaseIndentQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService)
        {
            _dbConnection = dbConnection;
            _ipAddressService = iPAddressService;
        }
        public async Task<(List<IndentHeader>, int)> GetAllPurchaseIndentAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var UnitId = _ipAddressService.GetUnitId();
              const string dataQuery = @" SELECT 
                IH.Id, 
                IH.IndentNumber,
                IH.IndentDate,
                IH.IndentTypeId,
                IH.UnitId,
                IH.Purpose,
                IH.IsActive,IH.CreatedDate,IH.CreatedBy,IH.CreatedByName,IH.ModifiedBy,IH.ModifiedDate,IH.ModifiedByName,IndentType.Id,IndentType.Code
            FROM [Purchase].[IndentHeader] IH
            INNER JOIN [Purchase].[MiscMaster] IndentType on IH.IndentTypeId=IndentType.Id
            WHERE 
            IH.IsDeleted = 0
                AND (@Search IS NULL OR IndentType.Code LIKE @Search OR IH.IndentNumber LIKE @Search)
                AND  IH.UnitId=@UnitId
                ORDER BY IH.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";
              const string countQuery = @"
              SELECT COUNT(*) 
               FROM [Purchase].[IndentHeader] IH
            INNER JOIN [Purchase].[MiscMaster] IndentType on IH.IndentTypeId=IndentType.Id
            WHERE 
            IH.IsDeleted = 0
                AND (@Search IS NULL OR IndentType.Code LIKE @Search OR IH.IndentNumber LIKE @Search)
                AND  IH.UnitId=@UnitId;
          ";


            var parameters = new
            {
                Search = string.IsNullOrEmpty(SearchTerm) ? null : $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize,
                UnitId
            };

            var Indent = await _dbConnection.QueryAsync<IndentHeader, Core.Domain.Entities.MiscMaster, IndentHeader>(
                dataQuery,
                (indentHeader, indentType) =>
                {
                     indentHeader.IndentType = new Core.Domain.Entities.MiscMaster
                     {
                         Id = indentType.Id,
                         Code = indentType.Code
                     };
                    
                     return indentHeader;
                },
                parameters,
                splitOn: "Id"                
                );
            
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);

            return (Indent.ToList(), totalCount);
        }

        public async Task<IndentHeader> GetByIdAsync(int id)
        {
             const string query = @"
                SELECT 
                IH.Id, 
                IH.IndentNumber,
                IH.IndentDate,
                IH.IndentTypeId,
                IH.UnitId,
                IH.Purpose,
                IH.IsActive,
                IDM.Id,IDM.IndentHeaderId,IDM.DepartmentId
            FROM [Purchase].[IndentHeader] IH
            INNER JOIN [Purchase].[IndentDepartmentMapping] IDM on IDM.IndentHeaderId=IH.Id
            WHERE IH.IsDeleted = 0 
              AND IH.IsActive = 1 AND IH.Id = @Id;";

              var IndentDictionary = new Dictionary<int, IndentHeader>();

            var IndentResponse = await _dbConnection.QueryAsync<IndentHeader, IndentDepartmentMapping, IndentHeader>(
                query,
                (indentHeader, indentDepart) =>
                {
                    if (!IndentDictionary.TryGetValue(indentHeader.Id, out var existingIndentHeader))
                    {
                        existingIndentHeader = indentHeader;
                        existingIndentHeader.IndentDepartmentMappings = new List<IndentDepartmentMapping>();
                        IndentDictionary[indentHeader.Id] = existingIndentHeader;
                    }

                    if (!existingIndentHeader.IndentDepartmentMappings!
                        .Any(a => a.Id == indentDepart.Id))
                    {
                        existingIndentHeader.IndentDepartmentMappings.Add(indentDepart);
                    }

                   

                    return existingIndentHeader;
                },
                new { id },
                splitOn: "Id"
                );

            return IndentResponse.FirstOrDefault()!;
        }

        public async Task<bool> NotFoundAsync(int id)
        {
             var query = "SELECT COUNT(1) FROM [Purchase].[IndentHeader]  WHERE Id = @Id AND IsDeleted = 0";
             
                var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Id = id });
                return count > 0;
        }
    }
}