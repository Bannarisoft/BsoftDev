using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDashboard;
using Core.Application.Dashboard.Common;
using MediatR;

namespace Core.Application.Dashboard
{
    public class DashboardQueryHandler : IRequestHandler<DashboardQuery, ChartDto>
    {
        private readonly IDashboardQueryRepository _repository;
        public DashboardQueryHandler(IDashboardQueryRepository repository)
        {
            _repository = repository;
        }
        public async Task<ChartDto> Handle(DashboardQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Type))
                throw new ArgumentException("Type is required. Valid values: 'assetexpirySummary'");

            return request.Type switch
            {
                "assetexpirySummary" => await _repository.GetAssetExpiredDashBoardDataAsync(),
                _ => throw new ArgumentException("Invalid type.")
            };
        }

    }
}