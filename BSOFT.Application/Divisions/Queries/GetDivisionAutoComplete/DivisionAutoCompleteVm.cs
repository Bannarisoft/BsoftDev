using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class DivisionAutoCompleteVm : IMapFrom<Division>
    {
        public int DivId { get; set; }
        public string Name { get; set; }
    }
}