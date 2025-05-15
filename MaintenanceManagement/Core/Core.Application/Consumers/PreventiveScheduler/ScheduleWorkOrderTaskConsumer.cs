using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Contracts.Events.Maintenance.PreventiveScheduler;
using Core.Application.Common.Interfaces.IBackgroundService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using Hangfire;
using MassTransit;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.Consumers.PreventiveScheduler
{
    public class ScheduleWorkOrderTaskConsumer : IConsumer<SheduleWorkOrderCommand>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        public ScheduleWorkOrderTaskConsumer(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        IMiscMasterQueryRepository miscMasterQueryRepository, IWorkOrderCommandRepository workOrderRepository, IMapper mapper, IBackgroundServiceClient backgroundServiceClient)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _backgroundServiceClient = backgroundServiceClient;
        }

        public async Task Consume(ConsumeContext<SheduleWorkOrderCommand> context)
        {
            try{
            CancellationToken cancellationToken = context.CancellationToken;
            var getMachineWiseDetail = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(context.Message.PreventiveSchedulerHeaderId);
            var getWorkOrderDetail = await _preventiveSchedulerQuery.GetByIdAsync(context.Message.PreventiveSchedulerHeaderId);
            
            var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);
                     foreach (var detail in getMachineWiseDetail)
                     {
                         var startDateTime = detail.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue);
                         var delay = startDateTime  - DateTime.Now;
                           string newJobId;
                         var delayInMinutes = (int)delay.TotalMinutes;
                             var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(getWorkOrderDetail, opt =>
                              {
                                  opt.Items["StatusId"] = miscdetail.Id;
                                  opt.Items["PreventiveSchedulerDetailId"] = detail.Id;
                              });
                         if (delay.TotalSeconds > 0)
                        {
                            newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detail.Id,delayInMinutes);
                        }
                        else
                        {
                        
                           newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detail.Id,5);
                        }
                          detail.HangfireJobId = newJobId;
                        await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id,newJobId);
                     }

                     if(getMachineWiseDetail.Count > 0)
                     {
                         await context.Publish(new ScheduleWorkOrderCreationEvent
                         {
                             CorrelationId = context.Message.CorrelationId
                         });
                     }
                       await context.Publish(new ScheduleWorkOrderFailedEvent
                         {
                             CorrelationId = context.Message.CorrelationId
                         });
             }
            catch (Exception ex)
            {
                await context.RespondAsync(new ScheduleWorkOrderFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}