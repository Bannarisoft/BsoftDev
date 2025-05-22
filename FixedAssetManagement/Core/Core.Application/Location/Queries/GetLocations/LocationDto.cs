using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Location.Queries.GetLocations
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? LocationName { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int UnitId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Status IsActive { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedByName { get; set; }

    }
}