using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MaintenanceType :BaseEntity
    {
        public string? TypeName { get; set; }
    }
}