using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyGroup.Queries.GetPartyGroup;
using MediatR;

namespace Core.Application.PartyGroup.Queries.GetPartyGroupById
{
    public class GetPartyGroupByIdQuery : IRequest<PartyGroupByIdDto>
    {
        public int Id { get; set; }
    }
}