using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Contracts.Events.Maintenance.PreventiveScheduler;
using MassTransit;
using SagaOrchestrator.Application.Orchestration.Models.PreventiveSchedule;

namespace SagaOrchestrator.Application.StateMachines
{
    public class PreventiveSchedulerStateMachine : MassTransitStateMachine<PreventiveSchedulerState>
    {
        public State CreatingPreventiveSchedulerDetail { get; private set; }
        public State CreatingScheduleWorkOrder { get; private set; }
        public State Failed { get; private set; }

        public Event<PreventiveSchedulerHeaderCreationEvent> HeaderCreatedEvent  { get; private set; }
        public Event<MachineWiseScheduleCreationEvent> DetailCreationEvent { get; private set; }
        public Event<PreventiveSchedulerDetailCreationFailedEvent> DetailFailedEvent { get; private set; }
        public Event<ScheduleWorkOrderCreationEvent> ScheduleWorkOrderEvent { get; private set; }
        public Event<ScheduleWorkOrderFailedEvent> ScheduleWorkOrderFailedEvent { get; private set; }
        public PreventiveSchedulerStateMachine()
        {
            InstanceState(x => x.CurrentState);
             Event(() => HeaderCreatedEvent, x =>
            {
                x.CorrelateById(context => context.Message.CorrelationId);
                x.InsertOnInitial = true;
             
            });
            Event(() => DetailCreationEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => DetailFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => ScheduleWorkOrderEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
            Event(() => ScheduleWorkOrderFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

               Initially(
                When(HeaderCreatedEvent)
                    .Then(context =>
                    {
                        context.Instance.PreventiveSchedulerHeaderId = context.Data.PreventiveSchedulerHeaderId;
                        context.Instance.MaintenanceCategoryId = context.Data.MaintenanceCategoryId;
                        context.Instance.MachineGroupId = context.Data.MachineGroupId;
                        context.Instance.FrequencyUnitId = context.Data.FrequencyUnitId;
                        context.Instance.EffectiveDate = context.Data.EffectiveDate;
                        context.Instance.FrequencyInterval = context.Data.FrequencyInterval;
                        context.Instance.ReminderWorkOrderDays = context.Data.ReminderWorkOrderDays;
                        context.Instance.ReminderMaterialReqDays = context.Data.ReminderMaterialReqDays;
                        context.Instance.UnitId = context.Data.UnitId;
                        context.Instance.ScheduleId = context.Data.ScheduleId;
                        context.Instance.FrequencyTypeId = context.Data.FrequencyTypeId;
                        context.Instance.GraceDays = context.Data.GraceDays;
                        context.Instance.IsDownTimeRequired = context.Data.IsDownTimeRequired;
                        context.Instance.DownTimeEstimateHrs = context.Data.DownTimeEstimateHrs;
                    })
                    .Send(new Uri("queue:schedule-detail-task-queue"), context => new CreateShedulerDetailsCommand
                    {
                        CorrelationId = context.Instance.CorrelationId,
                        PreventiveSchedulerHeaderId = context.Instance.PreventiveSchedulerHeaderId,
                        MachineGroupId = context.Instance.MachineGroupId,
                        FrequencyUnitId = context.Instance.FrequencyUnitId,
                        EffectiveDate = context.Data.EffectiveDate,
                        FrequencyInterval = context.Data.FrequencyInterval,
                        ReminderWorkOrderDays = context.Data.ReminderWorkOrderDays,
                        ReminderMaterialReqDays = context.Data.ReminderMaterialReqDays,
                        UnitId = context.Data.UnitId,
                        ScheduleId = context.Data.ScheduleId,
                        FrequencyTypeId = context.Data.FrequencyTypeId,
                        GraceDays = context.Data.GraceDays,
                        IsDownTimeRequired = context.Data.IsDownTimeRequired,
                        DownTimeEstimateHrs = context.Data.DownTimeEstimateHrs
                    })
                    .TransitionTo(CreatingPreventiveSchedulerDetail)

            );
             During(CreatingPreventiveSchedulerDetail,
                When(DetailCreationEvent)
                      .Send(new Uri("queue:schedule-workorder-queue"), ctx => new SheduleWorkOrderCommand
                      {
                          CorrelationId = ctx.Instance.CorrelationId,
                          PreventiveSchedulerHeaderId = ctx.Instance.PreventiveSchedulerHeaderId
                      })
                 .TransitionTo(CreatingScheduleWorkOrder),
                     When(DetailFailedEvent)
                        .Send(ctx => new Uri("queue:rollback-scheduleHeader-queue"), ctx => new RollbackPreventiveCommand
                        {
                            CorrelationId = ctx.Data.CorrelationId,
                            PreventiveSchedulerHeaderId = ctx.Instance.PreventiveSchedulerHeaderId,
                            Reason = ctx.Data.Reason                        
                        })
                    .TransitionTo(Failed)

                    
            );
            During(CreatingScheduleWorkOrder,
               When(ScheduleWorkOrderEvent)
                   .Finalize(),

                When(ScheduleWorkOrderFailedEvent)
                .ThenAsync(async ctx =>
                {
                    
                    await ctx.Send(new Uri("queue:rollback-ScheduleWorkOrder-queue"), new RollBackScheduleWorkOrderCommand
                    {
                        CorrelationId = ctx.Data.CorrelationId,
                        PreventiveSchedulerHeaderId = ctx.Instance.PreventiveSchedulerHeaderId,
                        Reason = ctx.Data.Reason
                    });

                    
                    await ctx.Send(new Uri("queue:rollback-scheduleHeader-queue"), new RollbackPreventiveCommand
                    {
                        CorrelationId = ctx.Data.CorrelationId,
                        PreventiveSchedulerHeaderId = ctx.Instance.PreventiveSchedulerHeaderId,
                        Reason = "Rollback triggered after ScheduleWorkOrder failure"
                    });
                })
                           .TransitionTo(Failed)
           );

            SetCompletedWhenFinalized();
        }
    }
}