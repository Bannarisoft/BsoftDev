using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IPreventiveSchedulerLog;
using Core.Domain.Entities;
using MediatR;
using Newtonsoft.Json;

namespace Core.Application.PreventiveSchedulers.Commands.MapMachine
{
    public class MapMachineCommandHandler : IRequestHandler<MapMachineCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        private readonly IPreventiveScheduleLogService _preventiveScheduleLogService;
        public MapMachineCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        IMiscMasterQueryRepository miscMasterQueryRepository, IBackgroundServiceClient backgroundServiceClient, IPreventiveScheduleLogService preventiveScheduleLogService)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _backgroundServiceClient = backgroundServiceClient;
            _preventiveScheduleLogService = preventiveScheduleLogService;
        }
        public async Task<ApiResponseDTO<bool>> Handle(MapMachineCommand request, CancellationToken cancellationToken)
        {
            await _preventiveScheduleLogService.CaptureLogs(request.Id,null,"Link Machine",JsonConvert.SerializeObject(request));
            var PreventiveSchedule = await _preventiveSchedulerQuery.GetByIdAsync(request.Id);
            var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(PreventiveSchedule.FrequencyUnitId);

            var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.LastMaintenanceActivityDate.ToDateTime(TimeOnly.MinValue), PreventiveSchedule.FrequencyInterval, frequencyUnit.Code ?? "", PreventiveSchedule.ReminderWorkOrderDays);
            var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.LastMaintenanceActivityDate.ToDateTime(TimeOnly.MinValue), PreventiveSchedule.FrequencyInterval, frequencyUnit.Code ?? "", PreventiveSchedule.ReminderMaterialReqDays);

            var dto = new MapPreventiveScheduleDetailDto
            {
                PreventiveSchedulerHeaderId = PreventiveSchedule.Id,
                MachineId = request.MachineId,
                WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate),
                ActualWorkOrderDate = DateOnly.FromDateTime(nextDate),
                MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate),
                ScheduleId = PreventiveSchedule.ScheduleId,
                FrequencyTypeId = PreventiveSchedule.FrequencyTypeId,
                FrequencyInterval = PreventiveSchedule.FrequencyInterval,
                FrequencyUnitId = PreventiveSchedule.FrequencyUnitId,
                GraceDays = PreventiveSchedule.GraceDays,
                ReminderWorkOrderDays = PreventiveSchedule.ReminderWorkOrderDays,
                ReminderMaterialReqDays = PreventiveSchedule.ReminderMaterialReqDays,
                IsDownTimeRequired = PreventiveSchedule.IsDownTimeRequired,
                DownTimeEstimateHrs = PreventiveSchedule.DownTimeEstimateHrs,
                LastMaintenanceActivityDate = request.LastMaintenanceActivityDate
            };
            var preventiveScheduler = _mapper.Map<PreventiveSchedulerDetail>(dto);
            var Preventiveresult = await _preventiveSchedulerCommand.CreateDetailAsync(preventiveScheduler);

            // Preventiveresult.Id
            int jobDelayMin = 0;
            var startDateTime = Preventiveresult.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue);

            var delay = startDateTime - DateTime.Today;
            string newJobId;
            var delayInMinutes = (int)delay.TotalMinutes;

            if (delay.TotalSeconds > 0)
            {
                newJobId = await _backgroundServiceClient.ScheduleWorkOrder(Preventiveresult.Id, delayInMinutes);
            }
            else
            {
                jobDelayMin += 2;

                newJobId = await _backgroundServiceClient.ScheduleWorkOrder(Preventiveresult.Id, jobDelayMin);
            }

            await _preventiveSchedulerCommand.UpdateDetailAsync(Preventiveresult.Id, newJobId);

            

             return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "New machine mapped successfully"
                    };
        }
    }
}