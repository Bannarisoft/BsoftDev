
using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Common;
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
    
        public async Task<(List<DepreciationDto>, int)> CalculateDepreciationAsync(int companyId,int unitId,int finYearId, DateTimeOffset? startDate, DateTimeOffset? endDate,int depreciationType, int PageNumber, int PageSize, string? SearchTerm,int depreciationPeriod)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYearId);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);
            parameters.Add("@Type", depreciationType);            
            parameters.Add("@Save", 0);
            parameters.Add("@Period", depreciationPeriod);            
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

        public async Task<(string message, int statusCode)>  CreateAsync(int companyId, int unitId, int finYearId,  int depreciationType,int depreciationPeriod)
        {
            int userId = _ipAddressService.GetUserId();
            string username = _ipAddressService.GetUserName();
            string ipAddress = _ipAddressService.GetSystemIPAddress();

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYearId);
            parameters.Add("@StartDate", null);
            parameters.Add("@EndDate", null);
            parameters.Add("@Type", depreciationType);
            parameters.Add("@Save", 1);
            parameters.Add("@Period", depreciationPeriod);            
            parameters.Add("@PageNumber", 0);
            parameters.Add("@PageSize", 0);
            parameters.Add("@SearchTerm", "");
            parameters.Add("@AssetId", 0);
            parameters.Add("@CreatedBy",userId);
            parameters.Add("@CreatedByName",  username);
            parameters.Add("@CreatedIP", ipAddress);

           // Execute the stored procedure (ignoring return message)
     /*      var result =  await _dbConnection.ExecuteAsync(
                "dbo.FAM_DepreciationCalculation",
                parameters,
                commandType: CommandType.StoredProcedure
            );    */
           var result = await _dbConnection.QueryAsync(
                "dbo.FAM_DepreciationCalculation",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Assuming your stored procedure returns 'Status' and 'StatusCode'
            var firstResult = result.FirstOrDefault();
            if (firstResult != null)
            {
                return (firstResult.Status, firstResult.StatusCode);
            }

            return ("Unknown error", -1);
        } 

        public async Task<bool> ExistDataAsync(int companyId, int unitId, int finYearId, int depreciationType,int depreciationPeriod)
        {
            return await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                            d.Finyear == finYearId &&
                            d.DepreciationPeriod == depreciationPeriod &&
                            d.DepreciationType == depreciationType &&                                                    
                            (unitId == 0 || d.UnitId == unitId)) 
                .AnyAsync();
        }
        public async Task<bool> ExistDataLockedAsync(int companyId, int unitId, int finYearId, int depreciationType,int depreciationPeriod)
        {
            return await _applicationDbContext.DepreciationDetails
                .Where(d => d.CompanyId == companyId &&
                            d.Finyear == finYearId &&
                            d.DepreciationPeriod == depreciationPeriod &&
                            d.DepreciationType == depreciationType &&  d.IsLocked ==1 &&                                             
                            (unitId == 0 || d.UnitId == unitId)) 
                .AnyAsync();
        }

        public async Task<List<DepreciationAbstractDto>> GetDepreciationAbstractAsync(int companyId, int unitId, int finYearId, DateTimeOffset? startDate,DateTimeOffset? endDate,int depreciationPeriod, int depreciationType)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@Finyear", finYearId);
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);
            parameters.Add("@Type", depreciationType);     
            parameters.Add("@Save", 0); 
            parameters.Add("@Period", depreciationPeriod);
            parameters.Add("@PageNumber", 0);     
            parameters.Add("@PageSize", 0);     
            parameters.Add("@SearchTerm", "");     
            parameters.Add("@AssetId", 0);     
            parameters.Add("@CreatedBy", 0);     
            parameters.Add("@CreatedByName", "");     
            parameters.Add("@CreatedIP", "");
            parameters.Add("@Consolidate", 1);                                

            var result = await _dbConnection.QueryAsync<DepreciationAbstractDto>("dbo.FAM_DepreciationCalculation", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationMethodAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.DeprecationPeriod.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
    }
}