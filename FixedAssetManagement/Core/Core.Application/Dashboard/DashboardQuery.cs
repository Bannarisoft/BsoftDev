using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Dashboard.Common;
using MediatR;

namespace Core.Application.Dashboard
{
    public class DashboardQuery : IRequest<ChartDto>
    {
         public string? Type { get; set; }
         
    }
}