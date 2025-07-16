using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.CostCenter.Queries.GetCostCenter;
using MediatR;

namespace Core.Application.CostCenter.Queries.GetCostCenterAutoComplete
{
    public class GetCostCenterAutoCompleteQuery : IRequest<List<CostCenterAutoCompleteDto>>
    {
         public string? SearchPattern { get; set; }
    }
}