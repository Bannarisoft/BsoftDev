using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Divisions.Queries.GetDivisionAutoComplete
{
    public class DivisionAutoCompleteVm : IMapFrom<Division>
    {
        public int DivId { get; set; }
        public string Name { get; set; }
    }
}