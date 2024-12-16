using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Units.Queries.GetUnitAutoComplete
{
    public class UnitAutoCompleteVm : IMapFrom<Unit>
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }
}