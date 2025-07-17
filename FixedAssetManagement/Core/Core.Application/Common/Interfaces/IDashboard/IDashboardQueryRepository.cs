using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Dashboard.CardView;

namespace Core.Application.Common.Interfaces.IDashboard
{
    public interface IDashboardQueryRepository
    {
         Task<AssetDashboardDto> GetDashboardDataAsync();
    }
}