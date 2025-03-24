using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterAutoComplete
{
    public class GetShiftMasterAutoCompleteQuery : IRequest<ApiResponseDTO<List<ShiftMasterAutoCompleteDTO>>>
    {
        public string SearchPattern { get; set; }
    }
}