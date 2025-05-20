
using System.Data;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IReports;
using Core.Application.Reports.AssetReport;
using Core.Application.Reports.AssetTransferReport;
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

        public async Task<List<AssetTransferDetailsDto>> AssetTransferReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            var sql = @"
                SELECT * FROM vw_AssetTransferStatus
                WHERE (@FromDate IS NULL OR DocDate >= @FromDate)
                AND (@ToDate IS NULL OR DocDate <= @ToDate)
                AND FromUnitId = @UnitId";

            var parameters = new
            {
                FromDate = fromDate,
                ToDate = toDate,
                UnitId = UnitId // <-- FIXED: must match the SQL parameter name
            };

            var result = await _dbConnection.QueryAsync<AssetTransferDetailsDto>(sql, parameters);
            return result.ToList();
        }
    }
}