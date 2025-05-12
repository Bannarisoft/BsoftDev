using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IBackgroundService;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using Hangfire;
using MediatR;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class UpdatePreventiveSchedulerCommandHandler : IRequestHandler<UpdatePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IIPAddressService _ipAddressService;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        public UpdatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator, IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveSchedulerQuery preventiveSchedulerQuery, IWorkOrderCommandRepository workOrderRepository, IIPAddressService ipAddressService, ITimeZoneService timeZoneService,IBackgroundServiceClient backgroundServiceClient)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _workOrderRepository = workOrderRepository;
            _ipAddressService = ipAddressService;
            _timeZoneService = timeZoneService;
            _backgroundServiceClient = backgroundServiceClient;
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
           
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

              var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(request.FrequencyUnitId);
                
                
                var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(request.Id);
                 foreach (var detail in DetailResult)
                 {
                            // var lastMaintenanceDate = await _preventiveSchedulerQuery.GetLastMaintenanceDateAsync(detail.MachineId);

                        //   DateTimeOffset baseDate = (!lastMaintenanceDate.HasValue || lastMaintenanceDate.Value < request.EffectiveDate.ToDateTime(TimeOnly.MinValue))
                        //    ? request.EffectiveDate.ToDateTime(TimeOnly.MinValue)
                        //         : lastMaintenanceDate.Value;

                            var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderWorkOrderDays);
                            var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderMaterialReqDays);

                     detail.PreventiveSchedulerHeaderId = request.Id;
                     detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                     detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                     detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);
                     detail.IsActive = preventiveScheduler.IsActive;

                      if (!string.IsNullOrEmpty(detail.HangfireJobId))
                     {
                         BackgroundJob.Delete(detail.HangfireJobId); 
                     }

                     
                 }
                 preventiveScheduler.PreventiveSchedulerDetails = DetailResult;
         
                var response  = await _preventiveSchedulerCommand.UpdateAsync(preventiveScheduler);
                var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);

                foreach (var detail in response.PreventiveSchedulerDetails)
                {
                    //   var workorderDocno =await _workOrderQueryRepository.GetLatestWorkOrderDocNo(preventiveScheduler.MaintenanceCategoryId);
                        var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(preventiveScheduler, opt =>
                        {
                            opt.Items["StatusId"] = miscdetail.Id;
                            // opt.Items["WorkOrderDocNo"] = workorderDocno;
                            opt.Items["PreventiveSchedulerDetailId"] = detail.Id;
                        });
               
                       string currentIp = _ipAddressService.GetSystemIPAddress();
                     int userId = _ipAddressService.GetUserId(); 
                     string username = _ipAddressService.GetUserName();
                     var systemTimeZoneId = _timeZoneService.GetSystemTimeZone();
                     var currentTime = _timeZoneService.GetCurrentTime(systemTimeZoneId);
                     
                     workOrderRequest.CreatedIP = currentIp;
                     workOrderRequest.CreatedDate = currentTime;
                     workOrderRequest.CreatedBy = userId;
                     workOrderRequest.CreatedByName = username;
                 
                       var delay = detail.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now;

                         string newJobId;
                         var delayInMinutes = (int)delay.TotalMinutes;
                        if (delay.TotalSeconds > 0)
                        {
                            
                            // newJobId =  BackgroundJob.Schedule(() => 
                            // _workOrderRepository.CreateAsync(workOrderRequest,preventiveScheduler.MaintenanceCategoryId, cancellationToken),
                            //  delay);
                            newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detail.Id,delayInMinutes);
                        }
                        else
                        {
                            
                            // newJobId =  BackgroundJob.Schedule(() => 
                            // _workOrderRepository.CreateAsync(workOrderRequest,preventiveScheduler.MaintenanceCategoryId, cancellationToken),
                            //  TimeSpan.FromMinutes(15));
                            newJobId =  await _backgroundServiceClient.ScheduleWorkOrder(detail.Id,5);
                        }
                        await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id,newJobId);
                }

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: "update",
                        actionName: "Update Preventive Scheduler",
                        details: $"Update Preventive Scheduler",
                        module:"Preventive Scheduler"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(response.Id > 0)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Preventive Scheduler updated successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Preventive Scheduler not updated."
                };
        }
    }
}