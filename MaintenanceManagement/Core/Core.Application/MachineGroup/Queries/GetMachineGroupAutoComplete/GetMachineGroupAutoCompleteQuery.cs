using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroup.Queries.GetMachineGroupAutoComplete
{
    public class GetMachineGroupAutoCompleteQuery :  IRequest<ApiResponseDTO<List<GetMachineGroupAutoCompleteDto>>>
    {
         public string? SearchPattern { get; set; }
    }
}