using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class UpdatePreventiveSchedulerCommandHandler : IRequestHandler<UpdatePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public UpdatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator, IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

              var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(request.FrequencyUnitId);
                
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderMaterialReqDays);
                var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(request.Id);
                 foreach (var detail in DetailResult)
                 {
                     detail.PreventiveSchedulerId = request.Id;
                     detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                     detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                     detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);
                     detail.IsActive = preventiveScheduler.IsActive;
                 }
                 preventiveScheduler.PreventiveSchedulerDetails = DetailResult;
         
                var response  = await _preventiveSchedulerCommand.UpdateAsync(preventiveScheduler);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: "update",
                        actionName: "Update Preventive Scheduler",
                        details: $"Update Preventive Scheduler",
                        module:"Preventive Scheduler"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(response)
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