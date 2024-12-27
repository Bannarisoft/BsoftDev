using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Units.Queries.GetUnitAutoComplete
{
    public class UnitAutoCompleteVm : IMapFrom<Unit>
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }
}