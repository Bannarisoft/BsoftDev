using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyGroup.Queries.GetPartyGroupAutoComplete;
using MediatR;

namespace Core.Application.PartyGroup.Queries.GetChildPartyGroupAutoComplete
{
    public class GetChildPartyGroupAutoCompleteQuery : IRequest<List<PartyGroupAutoCompleteDto>>
    {
        public string? SearchPattern { get; set; }
    }
}