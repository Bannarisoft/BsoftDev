using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Events;
using Contracts.Events.Maintenance;
using Contracts.Events.Users;
using MassTransit;
using Serilog;

namespace SagaOrchestrator.Application
{
    public class UserAssetState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }  // Holds the state int value (auto-managed)
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public int Version { get; set; }// Required by ISagaVersion
    }

    public class UserAssetStateMachine : MassTransitStateMachine<UserAssetState>
    {
        // States
        public State UserCreated { get; private set; }
        public State AssetAssigned { get; private set; }

        // Events
        public Event<UserCreatedEvent> UserCreatedEvent { get; private set; }
        public Event<AssetCreatedEvent> AssetCreatedEvent { get; private set; }
        public Event<SagaCompletedEvent> SagaCompletedEvent { get; private set; }

        public UserAssetStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UserCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AssetCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SagaCompletedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

            // Initial saga state handling
            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        Log.Information("🔥 [SAGA] UserCreatedEvent received - UserId: {UserId}, State: {State}",
                            context.Message.UserId, nameof(UserCreated));
                    })
                    .TransitionTo(UserCreated)
            );
            // Transition from UserCreated to AssetAssigned
            During(UserCreated,
                When(AssetCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.AssetId = context.Message.AssetId;
                        Log.Information("🛠️ [SAGA] AssetCreatedEvent received - AssetId: {AssetId}, AssetName: {AssetName}, State: {State}",
                            context.Message.AssetId, context.Message.AssetName, nameof(AssetAssigned));
                    })
                    .TransitionTo(AssetAssigned)
            );
            // Finalize saga on completion
            During(AssetAssigned,
                When(SagaCompletedEvent)
                    .Then(context =>
                    {
                        Log.Information("✅ [SAGA] SagaCompletedEvent received - Finalizing saga for UserId: {UserId}",
                            context.Message.UserId);
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}