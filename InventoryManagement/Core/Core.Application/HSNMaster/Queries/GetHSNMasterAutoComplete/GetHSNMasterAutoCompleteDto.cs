using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.HSNMaster.Queries.GetHSNMasterAutoComplete
{
    public class GetHSNMasterAutoCompleteDto
    {
        public int Id { get; set;}
        public string? HSNCode { get; set; }
        public string? HSNDescription { get; set; }
    }
}