using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.MaintenanceType.Queries.GetMaintenanceType
{
    public class MaintenanceTypeAutoCompleteDto
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }

    }
}