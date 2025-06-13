using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Commands.Maintenance.PreventiveScheduler.Update;
using Contracts.Events.Maintenance.PreventiveScheduler.PreventiveSchedulerUpdate;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MassTransit;

namespace Core.Application.Consumers.PreventiveScheduler.Update
{
    public class ScheduleWorkOrderConsumer : IConsumer<UpdateScheduleWorkOrderCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        public ScheduleWorkOrderConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMiscMasterQueryRepository miscMasterQueryRepository,
        IPreventiveSchedulerQuery preventiveSchedulerQuery, IBackgroundServiceClient backgroundServiceClient)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _backgroundServiceClient = backgroundServiceClient;
        }

        public async Task Consume(ConsumeContext<UpdateScheduleWorkOrderCommand> context)
        {
            try
            {
                var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(context.Message.FrequencyUnitId);

                var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);

                foreach (var detail in DetailResult)
                {

                    var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((detail.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
                     context.Message.FrequencyInterval, frequencyUnit.Code ?? "", context.Message.ReminderWorkOrderDays);
                    var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((detail.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue), context.Message.FrequencyInterval, frequencyUnit.Code ?? "", context.Message.ReminderMaterialReqDays);

                    detail.PreventiveSchedulerHeaderId = context.Message.PreventiveSchedulerHeaderId;
                    detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate);
                    detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                    detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

                    if (!string.IsNullOrEmpty(detail.HangfireJobId))
                    {
                        _backgroundServiceClient.RemoveHangFireJob(detail.HangfireJobId);
                    }

                    var delay = detail.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today;

                    string newJobId;
                    var delayInMinutes = (int)delay.TotalMinutes;
                    if (delay.TotalSeconds > 0)
                    {

                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detail.Id, delayInMinutes);
                    }
                    else
                    {

                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detail.Id, 5);
                    }
                    detail.HangfireJobId = newJobId;

                }
                await _preventiveSchedulerCommand.UpdateScheduleDetails(context.Message.PreventiveSchedulerHeaderId, DetailResult);
            }
            catch (Exception ex)
            {
                await context.Publish(new UpdateScheduleWorkOrderFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = "Failed to update schedule detail"
                        
                    });
            }
        }
    }
}