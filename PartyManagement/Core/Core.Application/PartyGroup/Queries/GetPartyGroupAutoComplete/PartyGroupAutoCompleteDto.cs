using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PartyGroup.Queries.GetPartyGroupAutoComplete
{
    public class PartyGroupAutoCompleteDto
    {
        public int Id { get; set; }
        public string? PartyGroupName { get; set; }
    }
}