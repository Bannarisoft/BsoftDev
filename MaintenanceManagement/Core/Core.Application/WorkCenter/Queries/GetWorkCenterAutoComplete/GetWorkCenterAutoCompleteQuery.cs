using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using MediatR;

namespace Core.Application.WorkCenter.Queries.GetWorkCenterAutoComplete
{
    public class GetWorkCenterAutoCompleteQuery : IRequest<ApiResponseDTO<List<WorkCenterAutoCompleteDto>>>
    {
        public string? SearchPattern { get; set; }
    }
}