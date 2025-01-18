using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Divisions.Queries.GetDivisions;
using MediatR;

namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class GetDivisionAutoCompleteQuery : IRequest<ApiResponseDTO<List<DivisionAutoCompleteDTO>>>
    {
        
        public string SearchPattern { get; set; }
    }
}