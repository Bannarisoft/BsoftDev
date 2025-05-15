using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Commands.Maintenance.PreventiveScheduler;
using Contracts.Events.Maintenance.PreventiveScheduler;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.Common.RealTimeNotificationHub;
using Core.Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Core.Application.Consumers.PreventiveScheduler
{
    public class ScheduleDetailCreateTaskConsumer : IConsumer<CreateShedulerDetailsCommand>
    {
        private readonly IPreventiveSchedulerCommand _nextScheduleService;
        private readonly IMachineMasterQueryRepository _machineMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IHubContext<PreventiveScheduleHub> _hubContext;
        public ScheduleDetailCreateTaskConsumer(IPreventiveSchedulerCommand nextScheduleService, IMachineMasterQueryRepository machineMasterQueryRepository, IMapper mapper,
        IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveSchedulerQuery preventiveSchedulerQuery, IWorkOrderCommandRepository workOrderRepository, IHubContext<PreventiveScheduleHub> hubContext)
        {
            _nextScheduleService = nextScheduleService;
            _machineMasterQueryRepository = machineMasterQueryRepository;
            _mapper = mapper;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _workOrderRepository = workOrderRepository;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<CreateShedulerDetailsCommand> context)
        {
              try
            {
                CancellationToken cancellationToken = context.CancellationToken;
                var machineMaster = await _machineMasterQueryRepository.GetMachineByGroupAsync(context.Message.MachineGroupId);
                
                var details = _mapper.Map<List<PreventiveSchedulerDetail>>(machineMaster);
                var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(context.Message.FrequencyUnitId);

                    // List<PreventiveSchedulerDetail> list = new List<PreventiveSchedulerDetail>();
                    foreach (var detail in details)
                     {
                    
                            var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(context.Message.EffectiveDate.ToDateTime(TimeOnly.MinValue), context.Message.FrequencyInterval, frequencyUnit.Code ?? "", context.Message.ReminderWorkOrderDays);
                            var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(context.Message.EffectiveDate.ToDateTime(TimeOnly.MinValue), context.Message.FrequencyInterval, frequencyUnit.Code ?? "", context.Message.ReminderMaterialReqDays);
    
                         detail.PreventiveSchedulerHeaderId = context.Message.PreventiveSchedulerHeaderId;
                         detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                         detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                         detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

                             var detailsResponse = await _nextScheduleService.CreateDetailAsync(detail);
                        // list.Add(detail);
                      }
                     
                    
                       
                if (details.Count > 0)
                {
                    await context.Publish(new MachineWiseScheduleCreationEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        PreventiveSchedulerHeaderId =context.Message.PreventiveSchedulerHeaderId
                        
                    });
                }
                else
                {
                     var headerId = context.Message.PreventiveSchedulerHeaderId;
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", 
                $"Preventive Schedule creation failed: {headerId}");
                    await context.Publish(new PreventiveSchedulerDetailCreationFailedEvent
                    {
                        CorrelationId = context.Message.CorrelationId,
                        Reason = "Failed to create schedule detail"
                    });
                }
            }
            catch (Exception ex)
            {
                var headerId = context.Message.PreventiveSchedulerHeaderId;
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", 
                $"Preventive Schedule creation failed: {headerId}");
                await context.RespondAsync(new PreventiveSchedulerDetailCreationFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Reason = $"Exception: {ex.Message}"
                });
            }
        }
    }
}