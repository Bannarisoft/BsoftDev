using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.CostCenter.Queries.GetCostCenter
{
    public class CostCenterDto
    {   
        public int Id { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public string? ResponsiblePerson { get; set; }
        public decimal? BudgetAllocated { get; set; }
        public string? Remarks { get; set; }
        public Status IsActive { get; set; }
        
    }
}