using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterAutoComplete
{
    public class ShiftMasterAutoCompleteDTO
    {
        public int Id { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
    }
}