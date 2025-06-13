using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Maintenance.PreventiveScheduler.Update;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using MassTransit;

namespace Core.Application.Consumers.PreventiveScheduler.Update
{
    public class RollBackScheduleWorkOrderConsumer : IConsumer<RollBackScheduleWorkOrderCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        public RollBackScheduleWorkOrderConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper,
        IPreventiveSchedulerQuery preventiveSchedulerQuery, IBackgroundServiceClient backgroundServiceClient, IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _backgroundServiceClient = backgroundServiceClient;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public async Task Consume(ConsumeContext<RollBackScheduleWorkOrderCommand> context)
        {
            var rollbackHeader = _mapper.Map<PreventiveSchedulerHeader>(context.Message.rollbackHeaders);
            await _preventiveSchedulerCommand.UpdateScheduleMetadata(rollbackHeader);

            var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);

            var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(rollbackHeader.FrequencyUnitId);
            foreach (var detail in DetailResult)
            {
                if (!string.IsNullOrEmpty(detail.HangfireJobId))
                {
                    _backgroundServiceClient.RemoveHangFireJob(detail.HangfireJobId);
                }

                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((detail.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
                    rollbackHeader.FrequencyInterval, frequencyUnit.Code ?? "", rollbackHeader.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((detail.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue), rollbackHeader.FrequencyInterval, frequencyUnit.Code ?? "", rollbackHeader.ReminderMaterialReqDays);

                
                detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate);
                detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

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
                 await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id, newJobId);
            }
           
        }
    }
}