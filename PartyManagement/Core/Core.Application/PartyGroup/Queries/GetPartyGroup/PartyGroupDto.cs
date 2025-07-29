using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.PartyGroup.Queries.GetPartyGroup
{
    public class PartyGroupDto
    {
        public int Id { get; set; }
        public string? PartyGroupName { get; set; }
        public int? ParentPartyGroupId { get; set; }
        public string? ParentPartyGroupName { get; set; }
        public int GroupTypeId { get; set; }
        public string? GroupName { get; set; }
        public string? Description { get; set; }
        public byte IsGroup { get; set; }
        public Status IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIp { get; set; }

        
    }
}