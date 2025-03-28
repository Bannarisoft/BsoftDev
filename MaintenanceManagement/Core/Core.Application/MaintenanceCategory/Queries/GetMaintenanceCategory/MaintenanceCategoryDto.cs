using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory
{
    public class MaintenanceCategoryDto
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public Status IsActive { get; set; }

    }
}