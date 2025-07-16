using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserAutoComplete
{
    public class GetMachineGroupUserAutoCompleteQuery : IRequest<List<MachineGroupUserAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; }
    }
}