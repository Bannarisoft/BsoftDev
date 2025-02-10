using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Location.Queries.GetLocations;
using MediatR;

namespace Core.Application.Location.Queries.GetLocationAutoComplete
{
    public class GetLocationAutoCompleteQuery : IRequest<ApiResponseDTO<List<LocationAutoCompleteDto>>>
    {
        public string SearchPattern { get; set; }
        public int UnitId { get; set; }
    }
}