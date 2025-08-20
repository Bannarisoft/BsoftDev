using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PartyMaster.Queries.GetPartMasterAutoComplete
{
    public class GetPartyMasterAutoCompleteDto
    {
        public int Id { get; set; }
        public string? PartyCode { get; set; }
        public string? PartyName { get; set; }
    }
}