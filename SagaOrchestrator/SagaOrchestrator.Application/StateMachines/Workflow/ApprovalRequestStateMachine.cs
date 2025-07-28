using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Workflow;
using Contracts.Events.Workflow;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.Workflow;

namespace SagaOrchestrator.Application.StateMachines.Workflow
{
    public class ApprovalRequestStateMachine : MassTransitStateMachine<ApprovalRequestState>
    {
        public State CreatingApprovalRequest { get; private set; }
        public State Failed { get; private set; }

        public Event<TransactionCreatedEvent> TransactionCreated { get; private set; }
        public Event<ApprovalRequestCreatedEvent> ApprovalRequestCreated { get; private set; }
        public Event<ApprovalRequestFailedEvent> ApprovalRequestFailed { get; private set; }

        public ApprovalRequestStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => TransactionCreated, x =>
           {
               x.CorrelateById(context => context.Message.CorrelationId);
               x.InsertOnInitial = true;

           });

            Initially(
                  When(TransactionCreated)
                      .Then(context =>
                      {
                          context.Saga.ModuleTypeName = context.Data.ModuleTypeName;
                          context.Saga.ModuleTransactionId = context.Data.ModuleTransactionId;
                          context.Saga.UnitId = context.Data.UnitId;
                          context.Saga.DepartmentId = context.Data.DepartmentId;
                      })
                      .Send(new Uri("queue:approval-request-task-queue"), context => new CreateApprovalRequestCommand
                      {
                          CorrelationId = context.Saga.CorrelationId,
                          ModuleTypeName = context.Saga.ModuleTypeName,
                          ModuleTransactionId = context.Saga.ModuleTransactionId,
                          UnitId = context.Saga.UnitId,
                          DepartmentId = context.Saga.DepartmentId
                      })
                      .TransitionTo(CreatingApprovalRequest)

              );
            During(CreatingApprovalRequest,
            When(ApprovalRequestCreated)
                .Finalize()

            //  When(ApprovalRequestFailed)
            //  .ThenAsync(async ctx =>
            //  {

            //     //  await ctx.Send(new Uri("queue:approval-request-rollback-queue"), new RollBackScheduleWorkOrderCommand
            //     //  {
            //     //      CorrelationId = ctx.Data.CorrelationId,
            //     //      Reason = ctx.Data.Reason,
            //     //      rollbackHeaders = ctx.Data.rollbackHeaders
            //     //  });
            //  })
            //  .TransitionTo(Failed)
        );

            SetCompletedWhenFinalized();
        }
    }
}