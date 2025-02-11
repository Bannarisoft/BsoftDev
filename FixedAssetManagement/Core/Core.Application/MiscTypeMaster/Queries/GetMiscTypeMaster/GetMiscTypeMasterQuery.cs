using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common.HttpResponse;

namespace Core.Application.MiscTypeMaster.Queries.GetMiscTypeMaster
{
    public class GetMiscTypeMasterQuery  : IRequest<ApiResponseDTO<List<GetMiscTypeMasterDto>>>
    {


         public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
        
    }
}