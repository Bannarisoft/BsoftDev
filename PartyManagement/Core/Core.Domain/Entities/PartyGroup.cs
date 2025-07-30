using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class PartyGroup : BaseEntity
    {
        public string? PartyGroupName { get; set; }
        public int? ParentPartyGroupId { get; set; }
        public PartyGroup? ParentPartyGroup { get; set; }
        public int GroupTypeId { get; set; } // Foreign key to MiscMaster
        public MiscMaster GroupType { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsGroup { get; set; }
        public ICollection<PartyGroup>? ChildPartyGroups { get; set; }

    }
}