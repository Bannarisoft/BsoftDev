using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Contracts.Events.Maintenance.PreventiveScheduler;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.Common.RealTimeNotificationHub;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.Consumers.PreventiveScheduler
{
    public class ScheduleWorkOrderTaskConsumer : IConsumer<SheduleWorkOrderCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        private readonly IHubContext<PreventiveScheduleHub> _hubContext;
        public ScheduleWorkOrderTaskConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        IMiscMasterQueryRepository miscMasterQueryRepository,  IMapper mapper, IBackgroundServiceClient backgroundServiceClient, IHubContext<PreventiveScheduleHub> hubContext)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _mapper = mapper;
            _backgroundServiceClient = backgroundServiceClient;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<SheduleWorkOrderCommand> context)
        {
            try{
            CancellationToken cancellationToken = context.CancellationToken;
            var getMachineWiseDetail = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);
            
                int jobDelayMin = 0;
                     foreach (var detail in getMachineWiseDetail)
                {


                    var startDateTime = detail.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue);
                    var delay = startDateTime - DateTime.Today;
                    string newJobId;
                    var delayInMinutes = (int)delay.TotalMinutes;
                    
                    if (delay.TotalSeconds > 0)
                    {
                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detail.Id, delayInMinutes);
                    }
                    else
                    {
                        jobDelayMin += 2;

                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detail.Id, jobDelayMin);
                    }
                    detail.HangfireJobId = newJobId;
                    await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id, newJobId);
                }
                var headerId = context.Message.PreventiveSchedulerHeaderId;
                if (getMachineWiseDetail.Count > 0)
                {

                    await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    $"Preventive Schedule created successfully: {headerId}");

                    await context.Publish(new ScheduleWorkOrderCreationEvent
                    {
                        CorrelationId = context.Message.CorrelationId
                    });
                }

                else
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    $"Preventive Schedule creation failed: {headerId}");

                    await context.Publish(new ScheduleWorkOrderFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId
                    });
                }
             }
            catch (Exception ex)
            {
                var headerId = context.Message.PreventiveSchedulerHeaderId;
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", 
                $"Preventive Schedule creation failed: {headerId}");

                await context.RespondAsync(new ScheduleWorkOrderFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}