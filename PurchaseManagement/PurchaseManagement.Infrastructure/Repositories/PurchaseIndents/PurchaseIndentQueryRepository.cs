using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IPurchaseIndent;
using Core.Domain.Entities;
using Dapper;

namespace PurchaseManagement.Infrastructure.Repositories.PurchaseIndents
{
    public class PurchaseIndentQueryRepository : IPurchaseIndentQuery,IPurchaseIndentGrpcQuery
    {
        private readonly IDbConnection _dbConnection;
        private readonly IIPAddressService _ipAddressService;
        private readonly IUnitGrpcClient _unitGrpcClient;
        public PurchaseIndentQueryRepository(IDbConnection dbConnection, IIPAddressService iPAddressService, IUnitGrpcClient unitGrpcClient)
        {
            _dbConnection = dbConnection;
            _ipAddressService = iPAddressService;
            _unitGrpcClient = unitGrpcClient;
        }

        public async Task<string> GeneratePurchaseIndentNumberAsync(int unitId)
        {

            string unitCode;
             var Units = await _unitGrpcClient.GetAllUnitAsync();
            var UnitLookup = Units.ToDictionary(d => d.UnitId, d => d.ShortName);

         
            if (UnitLookup.TryGetValue(unitId, out var ShortName))
            {
                 unitCode = ShortName;
            }
            else
            {
                throw new Exception("Invalid Unit Id. Failed to generate indent number.");
            }

               
               const string sql = @"
                   SELECT MAX(CAST(RIGHT(IndentNumber, 4) AS INT))
                   FROM Purchase.IndentHeader
                   WHERE UnitId = @UnitId 
                         ";

               int? maxSequence = await _dbConnection.ExecuteScalarAsync<int?>(sql, new
               {
                   UnitId = unitId
               });

                int newSequence = (maxSequence ?? 0) + 1;

               
               string indentNumber = $"PI/{unitCode}/{newSequence:D4}";

               return indentNumber;
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
                ID.Id,ID.IndentHeaderId,ID.ItemId,
                ID.QuantityRequired,ID.RequiredDate,ID.TotalEstimatedCost,ID.PRConsumptionDays,ID.Remark,ID.IsActive,
                IDM.Id,IDM.IndentHeaderId,IDM.DepartmentId
            FROM [Purchase].[IndentHeader] IH
            INNER JOIN [Purchase].[IndentDetail] ID on ID.IndentHeaderId=IH.Id
            INNER JOIN [Purchase].[IndentDepartmentMapping] IDM on IDM.IndentHeaderId=IH.Id
            WHERE IH.IsDeleted = 0 
              AND IH.IsActive = 1 AND IH.Id = @Id;";

              var IndentDictionary = new Dictionary<int, IndentHeader>();

            var IndentResponse = await _dbConnection.QueryAsync<IndentHeader, IndentDetail,IndentDepartmentMapping, IndentHeader>(
                query,
                (indentHeader, indentDetail,indentDepart) =>
                {
                    if (!IndentDictionary.TryGetValue(indentHeader.Id, out var existingIndentHeader))
                    {
                        existingIndentHeader = indentHeader;
                        existingIndentHeader.IndentDetails = new List<IndentDetail>();
                        existingIndentHeader.IndentDepartmentMappings = new List<IndentDepartmentMapping>();
                        IndentDictionary[indentHeader.Id] = existingIndentHeader;
                    }
                     if (!existingIndentHeader.IndentDetails!
                        .Any(a => a.Id == indentDetail.Id))
                    {
                        existingIndentHeader.IndentDetails.Add(indentDetail);
                    }

                    if (!existingIndentHeader.IndentDepartmentMappings!
                        .Any(a => a.Id == indentDepart.Id))
                    {
                        existingIndentHeader.IndentDepartmentMappings.Add(indentDepart);
                    }

                   

                    return existingIndentHeader;
                },
                new { id },
                splitOn: "Id,Id"
                );

            return IndentResponse.FirstOrDefault()!;
        }

        public async Task<IndentHeader> GetByIdGrpcAsync(int id)
        {
             const string query = @"
                SELECT 
                IH.Id, 
                IH.IndentNumber,
                IH.IndentDate,
                IH.IndentTypeId,
                IH.UnitId,
                IH.Purpose,
                IH.CreatedBy,IH.CreatedDate,
                ID.Id,ID.IndentHeaderId,ID.ItemId,
                ID.QuantityRequired,ID.RequiredDate,ID.TotalEstimatedCost,ID.PRConsumptionDays,ID.Remark,ID.IsActive,
                IDM.Id,IDM.IndentHeaderId,IDM.DepartmentId
            FROM [Purchase].[IndentHeader] IH
            INNER JOIN [Purchase].[IndentDetail] ID on ID.IndentHeaderId=IH.Id
            INNER JOIN [Purchase].[IndentDepartmentMapping] IDM on IDM.IndentHeaderId=IH.Id
            WHERE IH.IsDeleted = 0 
              AND IH.IsActive = 1 AND IH.Id = @Id;";

              var IndentDictionary = new Dictionary<int, IndentHeader>();

            var IndentResponse = await _dbConnection.QueryAsync<IndentHeader, IndentDetail,IndentDepartmentMapping, IndentHeader>(
                query,
                (indentHeader, indentDetail,indentDepart) =>
                {
                    if (!IndentDictionary.TryGetValue(indentHeader.Id, out var existingIndentHeader))
                    {
                        existingIndentHeader = indentHeader;
                        existingIndentHeader.IndentDetails = new List<IndentDetail>();
                        existingIndentHeader.IndentDepartmentMappings = new List<IndentDepartmentMapping>();
                        IndentDictionary[indentHeader.Id] = existingIndentHeader;
                    }
                     if (!existingIndentHeader.IndentDetails!
                        .Any(a => a.Id == indentDetail.Id))
                    {
                        existingIndentHeader.IndentDetails.Add(indentDetail);
                    }

                    if (!existingIndentHeader.IndentDepartmentMappings!
                        .Any(a => a.Id == indentDepart.Id))
                    {
                        existingIndentHeader.IndentDepartmentMappings.Add(indentDepart);
                    }

                   

                    return existingIndentHeader;
                },
                new { id },
                splitOn: "Id,Id"
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