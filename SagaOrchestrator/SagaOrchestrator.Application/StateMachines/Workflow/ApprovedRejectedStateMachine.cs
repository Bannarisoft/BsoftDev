using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Purchase;
using Contracts.Commands.Workflow;
using Contracts.Events.Purchase;
using Contracts.Events.Workflow;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.Workflow;

namespace SagaOrchestrator.Application.StateMachines.Workflow
{
    public class ApprovedRejectedStateMachine : MassTransitStateMachine<ApprovedRejectedState>
    {
        public State ApprovedRejectedRequest { get; private set; }
        public State Failed { get; private set; }

        public Event<ApprovedRejectedEvent> TransactionCreated { get; private set; }
        public Event<ApprovedRejectedFailedEvent> TransactionFailed { get; private set; }
        public ApprovedRejectedStateMachine()
        {
             InstanceState(x => x.CurrentState);
            Event(() => TransactionCreated, x =>
           {
               x.CorrelateById(context => context.Message.CorrelationId);
               x.InsertOnInitial = true;

           });
            Event(() => TransactionFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

            Initially(
                 When(TransactionCreated)
                     .Then(context =>
                     {
                         context.Saga.IndentId = context.Data.IndentId;
                         context.Saga.ApprovedQty = context.Data.ApprovedQty;
                     })
                     .Send(new Uri("queue:approved-rejected-task-queue"), context => new UpdateIndentDetailCommand
                     {
                         CorrelationId = context.Saga.CorrelationId,
                         IndentId = context.Saga.IndentId,
                         ApprovedQty = context.Saga.ApprovedQty
                     })
                     .TransitionTo(ApprovedRejectedRequest)

             );
             During(ApprovedRejectedRequest,
               When(TransactionCreated)
                   .Finalize(),

                When(TransactionFailed)
                .ThenAsync(async ctx =>
                {
                    
                    await ctx.Send(new Uri("queue:approved-rejected-rollback-queue"), new RollbackApprovedRejectStatus
                    {
                        CorrelationId = ctx.Data.CorrelationId,
                        IndentId = ctx.Data.IndentId,
                        Reason = ctx.Data.Reason,
                        RollbackApprovedRejected = ctx.Data.IndentDetails
                    });
                })
                .TransitionTo(Failed)
           );

            SetCompletedWhenFinalized();
        }
    }
}