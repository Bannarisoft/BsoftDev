using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.SubLocation.Queries.GetSubLocations
{
    public class SubLocationDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SubLocationName { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int LocationId { get; set; }
        public Status IsActive { get; set; }
    }
}