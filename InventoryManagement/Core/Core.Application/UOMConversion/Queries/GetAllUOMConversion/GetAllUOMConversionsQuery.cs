using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.UOMConversion.Queries.GetAllUOMConversion
{
    public class GetAllUOMConversionsQuery : IRequest<ApiResponseDTO<List<UOMConversionDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
        
    }
}