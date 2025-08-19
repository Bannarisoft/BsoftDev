using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.RackMaster.Queries.GetRackMasterAutoComplete
{
    public class GetRackMasterAutoCompleteQuery : IRequest<List<GetRackMasterAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; }
       
    }
}