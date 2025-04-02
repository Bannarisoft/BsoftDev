using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Events.Maintenance
{
    public class DepartmentCreatedEvent
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}