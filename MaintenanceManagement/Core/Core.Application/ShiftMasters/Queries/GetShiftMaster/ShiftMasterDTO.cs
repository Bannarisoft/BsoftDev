using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ShiftMasters.Queries.GetShiftMaster
{
    public class ShiftMasterDTO
    {
        public int Id { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public DateOnly EffectiveDate { get; set; }
    }
}