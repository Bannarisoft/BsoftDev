using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PartyGroup.Command.CreatePartyGroup
{
    public class CreatePartyGroupCommand : IRequest<int>
    {
        public string? PartyGroupName { get; set; }
        public int? ParentPartyGroupId { get; set; }
        public int GroupTypeId { get; set; }
        public string? Description { get; set; }
        public byte IsGroup { get; set; }
    }
}