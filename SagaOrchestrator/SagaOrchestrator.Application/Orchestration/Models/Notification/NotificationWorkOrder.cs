using MassTransit;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace SagaOrchestrator.Application.Orchestration.Models.Notifications
{
    public class NotificationWorkOrder : SagaStateMachineInstance
    {
        [BsonId]
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Email Channel
        public bool EmailSent { get; set; }
        public bool EmailFailed { get; set; }

        // SMS Channel
        public bool SmsSent { get; set; }
        public bool SmsFailed { get; set; }

        // In-App Channel
        public bool InAppSent { get; set; }
        public bool InAppFailed { get; set; }

        public string? FailureReason { get; set; }

        public int UnitId { get; set; }
        public int EventTypeId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
    }
}
