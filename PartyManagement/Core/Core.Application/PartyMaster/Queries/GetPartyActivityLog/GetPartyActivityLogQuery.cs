using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PartyMaster.Queries.GetPartyActivityLog
{
    public class GetPartyActivityLogQuery : IRequest<List<PartyActivityDto>>
    {
        public int PartyId { get; set; }
    }
}