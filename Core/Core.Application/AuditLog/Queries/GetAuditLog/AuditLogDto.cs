using Core.Application.Common.Mappings;
using Core.Domain.Entities;

namespace Core.Application.AuditLog.Queries.GetAuditLog
{
    public class AuditLogDto : IMapFrom<AuditLogs>
    { 
        public string Id { get; set; } // Use string for ObjectId     
        public int UserId { get; set; }
        public string UserName { get; set; }=string.Empty;
        public string MachineName { get; set; }=string.Empty;
        public string IPAddress { get; set; }=string.Empty;
        public string OS { get; set; }=string.Empty;
        public string Browser { get; set; }=string.Empty;
        public string Action { get; set; }=string.Empty;
        public string Details { get; set; }=string.Empty;
        public string Module { get; set; }=string.Empty;
        public DateTime CreatedAt { get; set; }   
        //public byte  IsActive { get; set; }=1;
                   
    }
}