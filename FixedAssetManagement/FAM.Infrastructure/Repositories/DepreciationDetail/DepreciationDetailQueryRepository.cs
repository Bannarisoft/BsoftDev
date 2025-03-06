
using System.Data;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Dapper;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.DepreciationDetail
{
    public class DepreciationDetailQueryRepository : IDepreciationDetailQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ApplicationDbContext _applicationDbContext;
        public DepreciationDetailQueryRepository(IDbConnection dbConnection,ApplicationDbContext applicationDbContext)
        {
            _dbConnection = dbConnection;
            _applicationDbContext = applicationDbContext;
        }
    
        public async Task<(List<DepreciationDto>, int)> CalculateDepreciationAsync(int companyId,int unitId,string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType, int PageNumber, int PageSize, string? SearchTerm)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYear);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);
            parameters.Add("@Type", depreciationType);            
            parameters.Add("@Save", 0);
            parameters.Add("@Consolidate", 0);
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

        public async Task<string> CreateAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate, string depreciationType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYear);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);
            parameters.Add("@Type", depreciationType);
            parameters.Add("@Save", 1);
            parameters.Add("@Consolidate", 0);
            parameters.Add("@PageNumber", 0);
            parameters.Add("@PageSize", 0);
            parameters.Add("@SearchTerm", "");
            parameters.Add("@AssetId", 0);
            parameters.Add("@CreatedBy", _applicationDbContext.DepreciationDetails.FirstOrDefault()?.CreatedBy);
            parameters.Add("@CreatedByName",  _applicationDbContext.DepreciationDetails.FirstOrDefault()?.CreatedByName);
            parameters.Add("@CreatedIP", _applicationDbContext.DepreciationDetails.FirstOrDefault()?.CreatedIP);

            // Execute the stored procedure
            var result = await _dbConnection.ExecuteAsync(
                "dbo.FAM_DepriciationCalculation",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result > 0 ? "Depreciation details inserted successfully via stored procedure." 
                              : "No records were inserted.";
        }

        public async Task<bool> ExistDataAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate, string depreciationType)
        {
            return await _applicationDbContext.DepreciationDetails
                .AnyAsync(d => d.CompanyId == companyId &&
                               d.UnitId == unitId &&
                               d.Finyear == finYear &&
                               d.StartDate == startDate &&
                               d.EndDate == endDate && d.DepreciationType==depreciationType);
        }
    }
}