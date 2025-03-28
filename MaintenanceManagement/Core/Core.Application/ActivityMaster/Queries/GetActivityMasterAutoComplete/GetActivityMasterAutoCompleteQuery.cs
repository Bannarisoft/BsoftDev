using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetActivityMasterAutoComplete
{
    public class GetActivityMasterAutoCompleteQuery :  IRequest<ApiResponseDTO<List<GetActivityMasterAutoCompleteDto>>>
    {
         public string? SearchPattern { get; set; }
    }
}