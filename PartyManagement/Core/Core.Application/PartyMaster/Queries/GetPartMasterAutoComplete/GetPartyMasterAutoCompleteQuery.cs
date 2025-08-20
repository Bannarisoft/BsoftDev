using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PartyMaster.Queries.GetPartMasterAutoComplete
{
    public class GetPartyMasterAutoCompleteQuery : IRequest<List<GetPartyMasterAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; }
    }
}