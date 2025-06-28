
using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using Dapper;
using FAM.Infrastructure.Data;
using FAM.Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.WDVDepreciation
{
    public class WdvDepreciationQueryRepository : BaseQueryRepository, IWdvDepreciationQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ApplicationDbContext _applicationDbContext;

        public WdvDepreciationQueryRepository(IDbConnection dbConnection, ApplicationDbContext applicationDbContext, IIPAddressService ipAddressService)
        : base(ipAddressService)
        {
            _dbConnection = dbConnection;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<CalculationDepreciationDto>> CalculateWDVAsync(int finYearId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);            
            parameters.Add("@FinYear", finYearId);    
            parameters.Add("@Flag", 3);         

            var result = await _dbConnection.QueryAsync<CalculationDepreciationDto>("dbo.FAM_WDVDepreciation", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<CalculationDepreciationDto>> CreateAsync(int finYearId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);            
            parameters.Add("@FinYear", finYearId);            
            parameters.Add("@Flag", 1); 
            parameters.Add("@CreatedBy", UserId); 
            parameters.Add("@CreatedByName", UserName); 
            parameters.Add("@CreatedIP", UserIPAddress); 

            var result = await _dbConnection.QueryAsync<CalculationDepreciationDto>("dbo.FAM_WDVDepreciation", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
       
         public async Task<List<CalculationDepreciationDto>> GetWDVDepreciationAsync(int finYearId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);            
            parameters.Add("@FinYear", finYearId);                        

            var result = await _dbConnection.QueryAsync<CalculationDepreciationDto>("dbo.FAM_WDVDepreciation", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<bool> ExistDataAsync(int finYearId)
        {
            return await _applicationDbContext.WDVDepreciationDetail
                .Where(d => d.CompanyId == CompanyId &&
                            d.FinYear == finYearId  ) 
                .AnyAsync();
        }

        public async Task<bool> ExistDataLockedAsync(int finYearId)
        {
             return await _applicationDbContext.WDVDepreciationDetail
                .Where(d => d.CompanyId == CompanyId &&
                            d.FinYear == finYearId  &&  d.IsLocked ==1)
                .AnyAsync();
        }      
    }
}
