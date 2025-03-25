using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroup.Quries.GetMachineGroupAutoComplete
{
    public class GetMiscMasterAutoCompleteQuery :  IRequest<ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>>>
    {
         public string? SearchPattern { get; set; }
    }
}