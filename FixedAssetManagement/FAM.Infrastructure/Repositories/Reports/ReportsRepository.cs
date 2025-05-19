
using System.Data;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Reports.AssetReport;
using Dapper;
using FAM.Infrastructure.Repositories.Common;

namespace FAM.Infrastructure.Repositories.Reports
{
    public class ReportsRepository : BaseQueryRepository,IReportRepository
    {
        private readonly IDbConnection _dbConnection;        
        public ReportsRepository(IDbConnection dbConnection, IIPAddressService ipAddressService)
            : base(ipAddressService) 
        {
            _dbConnection = dbConnection;            
        }

        public async Task<List<AssetReportDto>> AssetReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", CompanyId);
            parameters.Add("@UnitId", UnitId);
            parameters.Add("@FromDate", fromDate);
            parameters.Add("@Todate", toDate);
          
            var result = await _dbConnection.QueryAsync<AssetReportDto>(
                "dbo.Rpt_AssetReport", 
                parameters, 
                commandType: CommandType.StoredProcedure,
                commandTimeout: 120);
                
            return result.ToList(); 
        }
    }
}