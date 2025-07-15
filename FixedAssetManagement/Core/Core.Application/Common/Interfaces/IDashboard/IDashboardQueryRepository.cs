using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Dashboard.AssetExpired;
using Core.Application.Dashboard.CardView;
using Core.Application.Dashboard.Common;

namespace Core.Application.Common.Interfaces.IDashboard
{
    public interface IDashboardQueryRepository
    {
        Task<AssetDashboardDto> GetDashboardDataAsync();
        Task<ChartDto> GetAssetExpiredDashBoardDataAsync();
    }
}