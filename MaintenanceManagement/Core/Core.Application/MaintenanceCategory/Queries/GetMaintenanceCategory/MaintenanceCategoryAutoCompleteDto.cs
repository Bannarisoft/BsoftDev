using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory
{
    public class MaintenanceCategoryAutoCompleteDto
    {
          public int Id { get; set; }
        public string? CategoryName { get; set; }
    }
}