
using System;
using Core.Domain.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Domain.Entities
{
    public class AuditLogs  : AuditLogEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }=string.Empty;       
        public string Action { get; set; }=string.Empty;
        public string Details { get; set; }=string.Empty;
        public string Module { get; set; }=string.Empty;
        
        //public byte  IsActive { get; set; }=1;
    }
}