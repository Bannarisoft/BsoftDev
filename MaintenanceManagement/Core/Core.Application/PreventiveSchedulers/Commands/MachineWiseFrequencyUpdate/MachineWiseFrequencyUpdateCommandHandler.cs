using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IPreventiveSchedulerLog;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.PreventiveSchedulers.Commands.MachineWiseFrequencyUpdate
{
    public class MachineWiseFrequencyUpdateCommandHandler : IRequestHandler<MachineWiseFrequencyUpdateCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveScheduleLogService _preventiveScheduleLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MachineWiseFrequencyUpdateCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        IBackgroundServiceClient backgroundServiceClient, IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveScheduleLogService preventiveScheduleLogService,
        IHttpContextAccessor httpContextAccessor)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _backgroundServiceClient = backgroundServiceClient;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveScheduleLogService = preventiveScheduleLogService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponseDTO<bool>> Handle(MachineWiseFrequencyUpdateCommand request, CancellationToken cancellationToken)
        {
            await _preventiveScheduleLogService.CaptureLogs(null,request.Id,"Machine Wise Frequency Update",JsonConvert.SerializeObject(request));

            var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetailById(request.Id);
            var token = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString();
            var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(DetailResult.FrequencyUnitId ?? 0);
            if (request.IsActive == 1)
            {
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.LastMaintenanceActivityDate.ToDateTime(TimeOnly.MinValue),
                                request.FrequencyInterval, frequencyUnit.Code ?? "", DetailResult.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.LastMaintenanceActivityDate.ToDateTime(TimeOnly.MinValue),
                         request.FrequencyInterval, frequencyUnit.Code ?? "", DetailResult.ReminderMaterialReqDays);



                DetailResult.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                DetailResult.FrequencyInterval = request.FrequencyInterval;
                DetailResult.LastMaintenanceActivityDate = request.LastMaintenanceActivityDate;


                var result = await _preventiveSchedulerQuery.ExistWorkOrderBySchedulerDetailId(DetailResult.Id);

                if (result != true)
                {
                    DetailResult.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate);

                    DetailResult.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);
                    
                    // if (!string.IsNullOrEmpty(DetailResult.HangfireJobId))
                    // {
                    //     _backgroundServiceClient.RemoveHangFireJob(DetailResult.HangfireJobId,token);
                    // }

                    // var delay = DetailResult.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today;
                    var targetDateTime = DetailResult.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue);
                    var delay = targetDateTime - DateTime.Now;

                    string newJobId;
                    var delayInMinutes = (int)delay.TotalMinutes;
                    if (delay.TotalSeconds > 0)
                    {

                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(DetailResult.Id, delayInMinutes,token);
                    }
                    else
                    {

                        newJobId = await _backgroundServiceClient.ScheduleWorkOrder(DetailResult.Id, 5,token);
                    }
                    DetailResult.HangfireJobId = newJobId;
                }

            }
            else
            {
                DetailResult.IsActive = Status.Inactive;
                
                // if (!string.IsNullOrEmpty(DetailResult.HangfireJobId))
                //  {
                //      _backgroundServiceClient.RemoveHangFireJob(DetailResult.HangfireJobId,token);
                //  }
            }

            var response = await _preventiveSchedulerCommand.UpdateScheduleDetails(DetailResult);

             return new ApiResponseDTO<bool>
             {
                 IsSuccess = true, 
                 Message = "Preventive updated successfully"
             };
        }
    }
}