using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Interfaces.External.IMaintenance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.PreventiveSchedulers.Commands.MachineWiseFrequencyUpdate
{
    public class MachineWiseFrequencyUpdateCommandHandler : IRequestHandler<MachineWiseFrequencyUpdateCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IBackgroundServiceClient  _backgroundServiceClient;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        public MachineWiseFrequencyUpdateCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IPreventiveSchedulerQuery preventiveSchedulerQuery,
        IBackgroundServiceClient backgroundServiceClient, IMiscMasterQueryRepository miscMasterQueryRepository)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            _backgroundServiceClient = backgroundServiceClient;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }
        public async Task<ApiResponseDTO<bool>> Handle(MachineWiseFrequencyUpdateCommand request, CancellationToken cancellationToken)
        {
            var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetailById(request.Id);

            var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(DetailResult.FrequencyUnitId ?? 0);
            if (request.IsActive == 1)
            {
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((DetailResult.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
                           request.FrequencyInterval, frequencyUnit.Code ?? "", DetailResult.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate((DetailResult.LastMaintenanceActivityDate ?? DateOnly.FromDateTime(DateTime.Today)).ToDateTime(TimeOnly.MinValue),
                         request.FrequencyInterval, frequencyUnit.Code ?? "", DetailResult.ReminderMaterialReqDays);


                DetailResult.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate);
                DetailResult.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                DetailResult.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

                if (!string.IsNullOrEmpty(DetailResult.HangfireJobId))
                {
                    _backgroundServiceClient.RemoveHangFireJob(DetailResult.HangfireJobId);
                }

                var delay = DetailResult.WorkOrderCreationStartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today;

                string newJobId;
                var delayInMinutes = (int)delay.TotalMinutes;
                if (delay.TotalSeconds > 0)
                {

                    newJobId = await _backgroundServiceClient.ScheduleWorkOrder(DetailResult.Id, delayInMinutes);
                }
                else
                {

                    newJobId = await _backgroundServiceClient.ScheduleWorkOrder(DetailResult.Id, 5);
                }
                DetailResult.HangfireJobId = newJobId;
            }
            else
            {
                DetailResult.IsActive = Status.Active;
            }

            var response = await _preventiveSchedulerCommand.UpdateScheduleDetails(DetailResult);

             return new ApiResponseDTO<bool>
             {
                 IsSuccess = true, 
                 Message = "Preventive frequency updated successfully"
             };
        }
    }
}