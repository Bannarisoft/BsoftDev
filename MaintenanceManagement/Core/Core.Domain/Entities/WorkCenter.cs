using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class WorkCenter :BaseEntity
    {
        public string? WorkCenterCode { get; set; }
        public string? WorkCenterName { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
       
    }
}