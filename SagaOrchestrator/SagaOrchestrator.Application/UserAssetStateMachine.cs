using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaOrchestrator.Application
{
    public class UserAssetState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int CurrentState { get; set; }
        public Guid UserId { get; set; }
        public Guid AssetId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserAssetStateMachine : MassTransitStateMachine<UserAssetState>
    {
        public State UserCreated { get; private set; }
        public State AssetAssigned { get; private set; }
        public State AssetReleased { get; private set; }

        public Event<IUserCreated> UserCreatedEvent { get; private set; }
        public Event<IAssetAssigned> AssetAssignedEvent { get; private set; }
        public Event<IAssetReleased> AssetReleasedEvent { get; private set; }

        public UserAssetStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => UserCreatedEvent, x => x.CorrelateById(m => m.Message.UserId));
            Event(() => AssetAssignedEvent, x => x.CorrelateById(m => m.Message.UserId));
            Event(() => AssetReleasedEvent, x => x.CorrelateById(m => m.Message.UserId));

            Initially(
                When(UserCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.UserId = context.Message.UserId;
                        context.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .TransitionTo(UserCreated)
            );

            During(UserCreated,
                When(AssetAssignedEvent)
                    .Then(context =>
                    {
                        context.Saga.AssetId = context.Message.AssetId;
                    })
                    .TransitionTo(AssetAssigned)
            );

            During(AssetAssigned,
                When(AssetReleasedEvent)
                    .Then(context =>
                    {
                        context.Saga.AssetId = Guid.Empty;
                    })
                    .TransitionTo(AssetReleased)
            );
        }
    }
}