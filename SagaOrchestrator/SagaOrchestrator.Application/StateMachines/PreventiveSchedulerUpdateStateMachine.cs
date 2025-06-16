using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler.Update;
using Contracts.Events.Maintenance.PreventiveScheduler.PreventiveSchedulerUpdate;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.PreventiveSchedule;

namespace SagaOrchestrator.Application.StateMachines
{
    public class PreventiveSchedulerUpdateStateMachine : MassTransitStateMachine<PreventiveUpdateState>
    {
        public State CreatingScheduleWorkOrder { get; private set; }
        public State Failed { get; private set; }

        public Event<HeaderUpdateEvent> HeaderUpdateEvent { get; private set; }
        public Event<UpdateScheduleWorkOrderEvent> ScheduleWorkOrderEvent { get; private set; }
        public Event<UpdateScheduleWorkOrderFailedEvent> ScheduleWorkOrderFailedEvent { get; private set; }
        public PreventiveSchedulerUpdateStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Event(() => HeaderUpdateEvent, x =>
           {
               x.CorrelateById(context => context.Message.CorrelationId);
               x.InsertOnInitial = true;

           });
            Event(() => ScheduleWorkOrderEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => ScheduleWorkOrderFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

            Initially(
                 When(HeaderUpdateEvent)
                     .Then(context =>
                     {
                         context.Saga.PreventiveSchedulerHeaderId = context.Data.PreventiveSchedulerHeaderId;
                         //  context.Saga.MachineDetailDtos = context.Data.MachinedetailDtos;
                         context.Saga.UnitId = context.Data.UnitId;
                         context.Saga.FrequencyUnitId = context.Data.FrequencyUnitId;
                         context.Saga.FrequencyInterval = context.Data.FrequencyInterval;
                         context.Saga.ReminderWorkOrderDays = context.Data.ReminderWorkOrderDays;
                         context.Saga.ReminderMaterialReqDays = context.Data.ReminderMaterialReqDays;
                        context.Saga.rollbackHeaders = context.Data.rollbackHeaders;
                     })
                     .Send(new Uri("queue:update-scheduleWorkOrder-task-queue"), context => new UpdateScheduleWorkOrderCommand
                     {
                         CorrelationId = context.Saga.CorrelationId,
                         PreventiveSchedulerHeaderId = context.Saga.PreventiveSchedulerHeaderId,
                         //  MachinedetailDtos = context.Saga.MachineDetailDtos
                         UnitId = context.Saga.UnitId,
                         FrequencyUnitId = context.Saga.FrequencyUnitId,
                         FrequencyInterval = context.Data.FrequencyInterval,
                         ReminderWorkOrderDays = context.Data.ReminderWorkOrderDays,
                         ReminderMaterialReqDays = context.Data.ReminderMaterialReqDays,
                         rollbackHeaders = context.Data.rollbackHeaders
                     })
                     .TransitionTo(CreatingScheduleWorkOrder)

             );
             During(CreatingScheduleWorkOrder,
               When(ScheduleWorkOrderEvent)
                   .Finalize(),

                When(ScheduleWorkOrderFailedEvent)
                .ThenAsync(async ctx =>
                {
                    
                    await ctx.Send(new Uri("queue:update-rollback-scheduleWorkOrder-queue"), new RollBackScheduleWorkOrderCommand
                    {
                        CorrelationId = ctx.Data.CorrelationId,
                        PreventiveSchedulerHeaderId = ctx.Instance.PreventiveSchedulerHeaderId,
                        Reason = ctx.Data.Reason,
                        rollbackHeaders = ctx.Data.rollbackHeaders
                    });
                })
                .TransitionTo(Failed)
           );

            SetCompletedWhenFinalized();

        }
    }
}