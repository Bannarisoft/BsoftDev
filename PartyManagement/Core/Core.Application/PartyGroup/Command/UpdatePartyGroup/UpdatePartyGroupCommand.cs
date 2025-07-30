using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PartyGroup.Command.UpdatePartyGroup
{
    public class UpdatePartyGroupCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string? PartyGroupName { get; set; }
        public int? ParentPartyGroupId { get; set; }
        public string? Description { get; set; }
        public byte IsActive { get; set; }
    }
}