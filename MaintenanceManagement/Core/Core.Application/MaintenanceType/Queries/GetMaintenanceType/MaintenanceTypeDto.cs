using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MaintenanceType.Queries.GetMaintenanceType
{
    public class MaintenanceTypeDto
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public Status IsActive { get; set; }

        
    }
}