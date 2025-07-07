using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
// using Core.Application.Common.Interfaces.IBackgroundService;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Entities;
using Hangfire;
using MediatR;
using static Core.Domain.Common.BaseEntity;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.PreventiveSchedulers.Commands.ActiveInActivePreventive
{
    public class ActiveInActivePreventiveCommandHandler : IRequestHandler<ActiveInActivePreventiveCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMediator _mediator;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
          private readonly IMachineMasterQueryRepository _machineMasterQueryRepository;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IMapper _mapper;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        public ActiveInActivePreventiveCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMediator mediator, IPreventiveSchedulerQuery preventiveSchedulerQuery, IMachineMasterQueryRepository machineMasterQueryRepository, IMiscMasterQueryRepository miscMasterQueryRepository,IWorkOrderCommandRepository workOrderRepository,IMapper mapper,IBackgroundServiceClient backgroundServiceClient)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mediator = mediator;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _machineMasterQueryRepository = machineMasterQueryRepository;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _mapper =mapper;
            _backgroundServiceClient = backgroundServiceClient;
        }

        public async Task<ApiResponseDTO<bool>> Handle(ActiveInActivePreventiveCommand request, CancellationToken cancellationToken)
        {
            var Scheduledetail = await _preventiveSchedulerQuery.GetByIdAsync(request.Id);
            Scheduledetail.Id = 0;
            Scheduledetail.EffectiveDate = DateOnly.FromDateTime(DateTime.Today);

            
            foreach (var activity in Scheduledetail.PreventiveSchedulerActivities ?? new List<PreventiveSchedulerActivity>())
            {
                activity.Id = 0;
            }

            foreach (var item in Scheduledetail.PreventiveSchedulerItems ?? new List<PreventiveSchedulerItems>())
            {
                item.Id = 0;
            }
            
             
            if (request.IsActive == 1)
            {
                Scheduledetail.IsActive = Status.Active;

                 await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: "Schedule Active Header",
                     actionCode: "Active",
                     actionName: "Schedule Active Header Creation",
                     module: "Preventive",
                     requestData: Scheduledetail,
                     cancellationToken
                    );
                var response = await _preventiveSchedulerCommand.CreateAsync(Scheduledetail);

                var machineMaster = await _machineMasterQueryRepository.GetMachineByGroupAsync(Scheduledetail.MachineGroupId);

                var details = _mapper.Map<List<PreventiveSchedulerDetail>>(machineMaster);
                var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(Scheduledetail.FrequencyUnitId);

                var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode, StatusOpen.Code);
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(Scheduledetail.EffectiveDate.ToDateTime(TimeOnly.MinValue), Scheduledetail.FrequencyInterval, frequencyUnit.Code ?? "", Scheduledetail.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(Scheduledetail.EffectiveDate.ToDateTime(TimeOnly.MinValue), Scheduledetail.FrequencyInterval, frequencyUnit.Code ?? "", Scheduledetail.ReminderMaterialReqDays);

                foreach (var detail in details)
                {
                    detail.PreventiveSchedulerHeaderId = response;
                    detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate);
                    detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                    detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);
                    detail.ScheduleId = Scheduledetail.ScheduleId;
                     detail.FrequencyTypeId = Scheduledetail.FrequencyTypeId;
                     detail.FrequencyInterval = Scheduledetail.FrequencyInterval;
                     detail.FrequencyUnitId = Scheduledetail.FrequencyUnitId;
                     detail.GraceDays = Scheduledetail.GraceDays;
                     detail.ReminderWorkOrderDays = Scheduledetail.ReminderWorkOrderDays;
                     detail.ReminderMaterialReqDays = Scheduledetail.ReminderMaterialReqDays;
                     detail.IsDownTimeRequired = Scheduledetail.IsDownTimeRequired;
                     detail.DownTimeEstimateHrs = Scheduledetail.DownTimeEstimateHrs;

                    await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: "Schedule Active Detail",
                     actionCode: "Active",
                     actionName: "Schedule Active Detail Creation",
                     module: "Preventive",
                     requestData: detail,
                     cancellationToken
                    );

                    var detailsResponse = await _preventiveSchedulerCommand.CreateDetailAsync(detail);

                    // var workOrderRequest = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(Scheduledetail, opt =>
                    //  {
                    //      opt.Items["StatusId"] = miscdetail.Id;
                    //      opt.Items["PreventiveSchedulerDetailId"] = detailsResponse.Id;
                    //  });

                    var delay = reminderDate - DateTime.Today;

                    string newJobId;
                    var delayInMinutes = (int)delay.TotalMinutes;
                    if (delay.TotalSeconds > 0)
                    {
                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detailsResponse.Id, delayInMinutes);
                    }
                    else
                    {
                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(detailsResponse.Id, 5);
                    }

                    await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: $"Schedule WorkOrder delayInMinutes:{delayInMinutes} JobId:{newJobId}",
                     actionCode: "Schedule WorkOrder",
                     actionName: "Schedule WorkOrder",
                     module: "Preventive",
                     requestData: detailsResponse,
                     cancellationToken
                    );

                    await _preventiveSchedulerCommand.UpdateDetailAsync(detail.Id, newJobId);
                }
                await _preventiveSchedulerCommand.DeleteAsync(request.Id, Scheduledetail);
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "Preventive scheduler old one inactive and new one created successfully"
                };
            }
            else
            {
                 await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: $"Schedule Inactive",
                     actionCode: "Schedule Inactive",
                     actionName: "Schedule Inactive",
                     module: "Preventive",
                     requestData: request,
                     cancellationToken
                    );
                var preventiveSchedulerHeader = _mapper.Map<PreventiveSchedulerHeader>(request);
                await _preventiveSchedulerCommand.ScheduleInActive(preventiveSchedulerHeader);

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "Preventive scheduler inactive successfully"
                };
            }
        }
    }
}