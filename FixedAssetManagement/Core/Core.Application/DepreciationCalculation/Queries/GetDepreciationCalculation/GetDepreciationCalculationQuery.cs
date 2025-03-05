using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation
{
    public class GetDepreciationCalculationQuery : IRequest<ApiResponseDTO<List<DepreciationDto>>>
    {
        public int CompanyId {get; set;   }
        public int UnitId {get; set;   }
        public DateTimeOffset StartDate {get; set;}
        public DateTimeOffset EndDate {get; set;}
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}