using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.CostCenter.Queries.GetCostCenter
{
    public class CostCenterAutoCompleteDto
    {
        public int Id { get; set; }
        public string? CostCenterName { get; set; }

    }
}