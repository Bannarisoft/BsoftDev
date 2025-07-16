using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FAM.Infrastructure.Persistence
{
    public class OutboxMessage
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string? EventType { get; set; }
        public string? EventData { get; set; }
        public bool Processed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}