
using System.Data;
using Core.Application.Common.Interfaces.IDepreciationCalculation;
using Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation;
using Dapper;

namespace FAM.Infrastructure.Repositories.DepreciationCalculation
{
    public class DepreciationCalculationQueryRepository : IDepreciationCalculationQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public DepreciationCalculationQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
    
        public async Task<(List<DepreciationDto>, int)> CalculateDepreciationAsync(int CompanyId,int UnitId, DateTimeOffset StartDate, DateTimeOffset EndDate, int PageNumber, int PageSize, string? SearchTerm)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@StartDate", StartDate);
            parameters.Add("@EndDate", EndDate);
            parameters.Add("@PageNumber", PageNumber);
            parameters.Add("@PageSize", PageSize);
            parameters.Add("@SearchTerm", string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm);
            using var multiResult = await _dbConnection.QueryMultipleAsync(
            "dbo.FAM_DepriciationCalculation", parameters, commandType: CommandType.StoredProcedure);            
            // Read the first result set (Paginated Asset List)
            var assetMasterList = (await multiResult.ReadAsync<DepreciationDto>()).ToList();
            // Read the second result set (Total Record Count)
            int totalCount = await multiResult.ReadFirstAsync<int>();
            return (assetMasterList, totalCount);            
        }

        public Task<List<DepreciationDto>> GetByAssetNameAsync(string assetName)
        {
            throw new NotImplementedException();
        }

        public async Task<DepreciationDto> GetByIdAsync(int CompanyId,int UnitId, DateTimeOffset StartDate, DateTimeOffset EndDate, int PageNumber, int PageSize, string? SearchTerm,int assetId)
        {
             var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@StartDate", StartDate);
            parameters.Add("@EndDate", EndDate);
            parameters.Add("@PageNumber", PageNumber);
            parameters.Add("@PageSize", PageSize);
            parameters.Add("@SearchTerm", string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm);
            parameters.Add("@AssetId", assetId);
            using var multiResult = await _dbConnection.QueryMultipleAsync(
            "dbo.FAM_DepriciationCalculation", parameters, commandType: CommandType.StoredProcedure);                                   
            // Read a single result or return null if no data
            var result = (await multiResult.ReadAsync<DepreciationDto>()).FirstOrDefault();
            return result;   
        }
    }
}