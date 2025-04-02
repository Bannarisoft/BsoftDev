using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Events.Users;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MassTransit;
using MongoDB.Driver;
using Serilog;
using UserManagement.Infrastructure.Persistence;

namespace UserManagement.Infrastructure.Services
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IMongoCollection<OutboxMessage> _outboxCollection;
        private readonly IPublishEndpoint _publishEndpoint;
        public EventPublisher(IMongoCollection<OutboxMessage> outboxCollection, IPublishEndpoint publishEndpoint)
        {
            _outboxCollection = outboxCollection;
            _publishEndpoint = publishEndpoint;
        }
        public async Task SaveEventAsync<T>(T @event) where T : class
        {
            var message = new OutboxMessage
            {
                EventType = @event.GetType().Name,
                EventData = JsonSerializer.Serialize(@event),
                Processed = false,
                CreatedAt = DateTime.UtcNow
            };
            await _outboxCollection.InsertOneAsync(message);
        }
        public async Task PublishPendingEventsAsync()
        {
            var pendingMessages = await _outboxCollection.Find(x => !x.Processed).ToListAsync();

            foreach (var message in pendingMessages)
            {
                try
                {
                    var @event = JsonSerializer.Deserialize<UserCreatedEvent>(message.EventData);
                    await _publishEndpoint.Publish(@event);

                    message.Processed = true;
                    await _outboxCollection.ReplaceOneAsync(x => x.Id == message.Id, message);
                }
                catch (Exception ex)
                {
                    Log.Information($"Error publishing event: {ex.Message}");
                }
            }
        }


    }
}