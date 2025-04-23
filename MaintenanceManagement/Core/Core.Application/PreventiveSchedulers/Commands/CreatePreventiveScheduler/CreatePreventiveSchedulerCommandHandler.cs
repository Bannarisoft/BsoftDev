using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMiscMaster;

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class CreatePreventiveSchedulerCommandHandler : IRequestHandler<CreatePreventiveSchedulerCommand, ApiResponseDTO<int>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMachineMasterQueryRepository _machineMasterQueryRepository;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        
        public CreatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator, IMachineMasterQueryRepository machineMasterQueryRepository, IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _machineMasterQueryRepository = machineMasterQueryRepository;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
            
        }
        public async Task<ApiResponseDTO<int>> Handle(CreatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

                var response = await _preventiveSchedulerCommand.CreateAsync(preventiveScheduler);
                
               var machineMaster = await _machineMasterQueryRepository.GetMachineByGroupAsync(request.MachineGroupId);
                
                var details = _mapper.Map<List<PreventiveSchedulerDetail>>(machineMaster);
                var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(request.FrequencyUnitId);
                
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderMaterialReqDays);
                
                 foreach (var detail in details)
                 {
                     detail.PreventiveSchedulerId = response;
                     detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                     detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                     detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);
                 }
                 var detailsResponse = await _preventiveSchedulerCommand.CreateDetailAsync(details);
                 if(!detailsResponse)
                 {
                      await _preventiveSchedulerCommand.DeleteAsync(response,preventiveScheduler);
                     return new ApiResponseDTO<int>
                     {
                         IsSuccess = false, 
                         Message = "Preventive scheduler not created"
                     };
                 }
              
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "Create preventive scheduler",
                     actionName: "Create",
                     details: $"Preventive scheduler created",
                     module:"Preventive scheduler"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Preventive scheduler created successfully",
                         Data = response
                    };
                
        }
       
    }
}