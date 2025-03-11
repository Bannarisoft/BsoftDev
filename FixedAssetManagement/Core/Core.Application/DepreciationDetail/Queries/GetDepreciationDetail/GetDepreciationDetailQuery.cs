using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.DepreciationDetail.Queries.GetDepreciationDetail
{
    public class GetDepreciationDetailQuery : IRequest<ApiResponseDTO<List<DepreciationDto>>>
    {
        public int companyId {get; set;   }
        public int unitId {get; set;   }
        public string? finYear {get; set;}
        public DateTimeOffset? startDate {get; set;}
        public DateTimeOffset? endDate {get; set;}
        public string? depreciationType {get; set;}
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
        public int depreciationPeriod { get; set; }
    }
}