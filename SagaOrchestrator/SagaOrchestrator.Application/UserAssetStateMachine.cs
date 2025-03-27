using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
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

        public Event<UserCreatedEvent> UserCreatedEvent { get; private set; }
        public Event<IAssetAssigned> AssetAssignedEvent { get; private set; }

        public UserAssetStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UserCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AssetAssignedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        Console.WriteLine($"UserCreatedEvent received. UserId: {context.Message.UserId}");
                    })
                    .TransitionTo(UserCreated)
            );

            During(UserCreated,
                When(AssetAssignedEvent)
                    .Then(context =>
                    {
                        context.Saga.AssetId = context.Message.AssetId;
                        Console.WriteLine($"AssetAssignedEvent received. AssetId: {context.Message.AssetId}");
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}