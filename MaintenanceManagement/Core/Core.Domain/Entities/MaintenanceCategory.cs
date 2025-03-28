using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MaintenanceCategory : BaseEntity
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
    }
}