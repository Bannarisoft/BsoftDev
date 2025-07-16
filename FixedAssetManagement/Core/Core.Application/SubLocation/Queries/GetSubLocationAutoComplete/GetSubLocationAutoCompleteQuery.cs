using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Location.Queries.GetSubLocations;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocationAutoComplete
{
    public class GetSubLocationAutoCompleteQuery :  IRequest<ApiResponseDTO<List<SubLocationAutoCompleteDto>>>
    {
        public string? SearchPattern { get; set; }
        
    }
}