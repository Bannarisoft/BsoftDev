using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMasterAutoComplete
{
    public class GetMachineMasterAutoCompleteQuery : IRequest<List<MachineMasterAutoCompleteDto>>
    {
         public string? SearchPattern { get; set; }
    }
}