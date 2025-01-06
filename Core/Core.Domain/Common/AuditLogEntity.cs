using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Common
{
    public abstract class AuditLogEntity
    {
        public string MachineName { get; set; }=string.Empty;
        public string IPAddress { get; set; }=string.Empty;
        public string OS { get; set; }=string.Empty;
        public string Browser { get; set; }=string.Empty;
        public DateTime CreatedAt { get; set; }   
    }
}