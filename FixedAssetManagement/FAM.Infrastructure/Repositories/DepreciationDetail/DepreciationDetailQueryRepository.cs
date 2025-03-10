
using System.Data;
using Core.Application.Common.Interfaces;
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
        private readonly IIPAddressService _ipAddressService;
        public DepreciationDetailQueryRepository(IDbConnection dbConnection,ApplicationDbContext applicationDbContext,IIPAddressService ipAddressService)
        {
            _dbConnection = dbConnection;
            _applicationDbContext = applicationDbContext;
            _ipAddressService = ipAddressService;
        }
    
        public async Task<(List<DepreciationDto>, int)> CalculateDepreciationAsync(int companyId,int unitId,string finYear, DateTimeOffset? startDate, DateTimeOffset? endDate,string depreciationType, int PageNumber, int PageSize, string? SearchTerm,int depreciationPeriod)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYear);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);
            parameters.Add("@Type", depreciationType);            
            parameters.Add("@Save", 0);
            parameters.Add("@Period", depreciationPeriod);
            parameters.Add("@Consolidate", 0);    
            parameters.Add("@PageNumber", PageNumber );
            parameters.Add("@PageSize", PageSize );
            parameters.Add("@SearchTerm", SearchTerm);

                // ✅ Ensure using statement to properly handle GridReader disposal
            using var multiResult = await _dbConnection.QueryMultipleAsync(
                "dbo.FAM_DepreciationCalculation", parameters, commandType: CommandType.StoredProcedure);

            // ✅ Read all data before exiting the using block
            var depreciationList = (await multiResult.ReadAsync<DepreciationDto>()).ToList();
            int totalCount = await multiResult.ReadFirstOrDefaultAsync<int>();

            return (depreciationList, totalCount);   
        }

        public async Task<List<DepreciationDto>> CreateAsync(int companyId, int unitId, string finYear,  string depreciationType,int depreciationPeriod)
        {
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            string ipAddress = _ipAddressService.GetSystemIPAddress();

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYear);
            parameters.Add("@StartDate", null);
            parameters.Add("@EndDate", null);
            parameters.Add("@Type", depreciationType);
            parameters.Add("@Save", 1);
            parameters.Add("@Period", depreciationPeriod);
            parameters.Add("@Consolidate", 0);
            parameters.Add("@PageNumber", 0);
            parameters.Add("@PageSize", 0);
            parameters.Add("@SearchTerm", "");
            parameters.Add("@AssetId", 0);
            parameters.Add("@CreatedBy",userId);
            parameters.Add("@CreatedByName",  username);
            parameters.Add("@CreatedIP", ipAddress);

           // Execute the stored procedure (ignoring return message)
            await _dbConnection.ExecuteAsync(
                "dbo.FAM_DepreciationCalculation",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            
          //  return await CalculateDepreciationAsync(companyId, unitId, finYear, startDate, endDate, depreciationType, 0, 0, null);
             (List<DepreciationDto> depreciationList, _) = await CalculateDepreciationAsync(
             companyId, unitId, finYear, null, null, depreciationType,0,0, null,depreciationPeriod
    );
     return depreciationList;
        }
 
/*         public async Task<bool> DeleteLockedAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate, string depreciationType)
        {
           return await _applicationDbContext.DepreciationDetails
                .AnyAsync(d => d.CompanyId == companyId &&
                               d.UnitId == unitId &&
                               d.Finyear == finYear &&
                               d.StartDate == startDate &&
                               d.EndDate == endDate && d.DepreciationType==depreciationType && d.IsLocked == 1 && d.IsDeleted == 0);
        }  */

        public async Task<bool> ExistDataAsync(int companyId, int unitId, string finYear, string depreciationType,int depreciationPeriod)
        {
            return await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                            d.Finyear == finYear &&
                            d.DepreciationPeriod == depreciationPeriod &&
                            d.DepreciationType == depreciationType &&
                            d.IsLocked == 1 &&
                            d.IsDeleted == 0 &&
                            (unitId == 0 || d.UnitId == unitId)) 
                .AnyAsync();
        }
    }
}