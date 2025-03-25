using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.WorkCenter.Queries.GetWorkCenter
{
    public class WorkCenterDto
    {
        public int Id { get; set; }
        public string? WorkCenterCode { get; set; }
        public string? WorkCenterName { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public Status IsActive { get; set; }
    }
}