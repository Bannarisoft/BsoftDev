using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.UOM.Queries.GetUOMs;
using MediatR;

namespace Core.Application.UOM.Queries.GetUOMAutoComplete
{
    public class GetUOMAutoCompleteQuery: IRequest<ApiResponseDTO<List<UOMAutoCompleteDto>>>
    {
        public string? SearchPattern { get; set; }
        
    }
}