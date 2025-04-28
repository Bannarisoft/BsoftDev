using Contracts.Commands.Users;
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
        public Event<UserCreationFailedEvent> UserCreationFailedEvent { get; private set; }
        public Event<AssetCreationFailedEvent> AssetCreationFailedEvent { get; private set; }

        public UserAssetStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UserCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AssetCreatedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => SagaCompletedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => UserCreationFailedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));
            Event(() => AssetCreationFailedEvent, x => x.CorrelateById(m => m.Message.CorrelationId));

            // Initial saga state handling
            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        Log.Information("🔥 [SAGA] UserCreatedEvent received - UserId: {UserId}, State: {State}",
                            context.Message.UserId, nameof(UserCreated));
                    })
                    .TransitionTo(UserCreated),

                     When(UserCreationFailedEvent)
                    .Then(context =>
                    {
                        Log.Error("❌ [SAGA] User creation failed - Reason: {Reason}", context.Message.Reason);
                        // TODO: Send compensation commands if needed
                    })
                    .Finalize()
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
                    .TransitionTo(AssetAssigned),
                    
                     When(AssetCreationFailedEvent)
                    .Then(async context =>
                    {
                        Log.Error("❌ [SAGA] Asset creation failed - Reason: {Reason}", context.Message.Reason);
                        // TODO: Send compensation commands, e.g., delete user
                        // 🔁 Rollback by sending DeleteUserCommand
                        await context.Publish(new DeleteUserCommand
                        {
                            UserId = context.Saga.UserId,
                            Reason = $"Asset creation failed: {context.Message.Reason}"
                        });

                        Log.Warning("🧹 DeleteUserCommand published for rollback - UserId: {UserId}", context.Saga.UserId);
                    })
                    .Finalize()
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