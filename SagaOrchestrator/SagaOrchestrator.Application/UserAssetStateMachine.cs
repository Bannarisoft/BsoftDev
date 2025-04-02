using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
using Contracts.Events.Users;
using MassTransit;

namespace SagaOrchestrator.Application
{
    public class UserAssetState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public int UserId { get; set; }
        public int AssetId { get; set; }
        // public DateTime CreatedAt { get; set; } 
        // Required by ISagaVersion
        public int Version { get; set; }
    }

    public class UserAssetStateMachine : MassTransitStateMachine<UserAssetState>
    {
        public State UserCreated { get; private set; }
        public State AssetAssigned { get; private set; }

        public Event<Contracts.Events.Users.UserCreatedEvent> UserCreatedEvent { get; private set; }
        public Event<AssetCreatedEvent> AssetCreatedEvent { get; private set; }
        public Event<SagaCompletedEvent> SagaCompletedEvent { get; private set; }

        public UserAssetStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UserCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AssetCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SagaCompletedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        Console.WriteLine($"User Created: {context.Message.UserName}");
                    })
                    .TransitionTo(UserCreated)
            );

            During(UserCreated,
                When(AssetCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.AssetId = context.Message.AssetId;
                        Console.WriteLine($"Asset Assigned: {context.Message.AssetName}");
                    })
                    .TransitionTo(AssetAssigned)
            );

            During(AssetAssigned,
                When(SagaCompletedEvent)
                    .Then(context =>
                    {
                        Console.WriteLine($"Saga Completed for UserId: {context.Message.UserId}");
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}