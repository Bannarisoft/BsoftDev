using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Common
{
    public abstract class AuditLogBase
    {
        public string MachineName { get; set; }=string.Empty;
        public string IPAddress { get; set; }=string.Empty;
        public string OS { get; set; }=string.Empty;
        public string Browser { get; set; }=string.Empty;
        public DateTime CreatedAt { get; set; }   
        public int CreatedBy { get; set; }
        public String? CreatedByName { get; set; }
    }
}