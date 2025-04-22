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
using Contracts.Events.PreventScheduler;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using static Core.Domain.Common.MiscEnumEntity;
using Hangfire;
using Core.Application.Common.Interfaces.IWorkOrder;

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
            try{
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

                var response = await _preventiveSchedulerCommand.CreateAsync(preventiveScheduler);
                
               var machineMaster = await _machineMasterQueryRepository.GetMachineByGroupAsync(request.MachineGroupId);
                
                var details = _mapper.Map<List<PreventiveSchedulerDetail>>(machineMaster);
                var frequencyUnit = await _miscMasterQueryRepository.GetByIdAsync(request.FrequencyUnitId);
                
                var (nextDate, reminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderWorkOrderDays);
                var (ItemNextDate, ItemReminderDate) = await _preventiveSchedulerQuery.CalculateNextScheduleDate(request.EffectiveDate.ToDateTime(TimeOnly.MinValue), request.FrequencyInterval, frequencyUnit.Code ?? "", request.ReminderMaterialReqDays);
                var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);
                 foreach (var detail in details)
                 {
                     detail.PreventiveSchedulerId = response;
                     detail.WorkOrderCreationStartDate = DateOnly.FromDateTime(reminderDate); 
                     detail.ActualWorkOrderDate = DateOnly.FromDateTime(nextDate);
                     detail.MaterialReqStartDays = DateOnly.FromDateTime(ItemReminderDate);

                      
                        var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(preventiveScheduler);
               
                     workOrderRequest.StatusId = miscdetail.Id;
                 
                 var delay = reminderDate - DateTime.Now;

                   string newJobId;
                  if (delay.TotalSeconds > 0)
                  {
                      newJobId = BackgroundJob.Schedule<IWorkOrderCommandRepository>(
                          job => job.CreateAsync(workOrderRequest,cancellationToken),
                          delay
                      );
                  }
                  else
                  {
                      newJobId = BackgroundJob.Enqueue<IWorkOrderCommandRepository>(
                          job => job.CreateAsync(workOrderRequest,cancellationToken)
                          );
                  }
                  detail.HangfireJobId = newJobId;
                    //  {
                    //      PreventiveScheduleId = response,
                    //      StatusId = miscdetail.Id,
                    //      WorkOrderActivity = _mapper.Map<List<WorkOrderActivityDto>>(preventiveScheduler.PreventiveSchedulerActivities),
                    //      WorkOrderItem = _mapper.Map<List<WorkOrderItemDto>>(preventiveScheduler.PreventiveSchedulerItems)
                    //  };
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
                    catch (AutoMapperMappingException ex)
                    {
                        Console.WriteLine("AutoMapper Error: " + ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("Inner: " + ex.InnerException.Message);
                            throw new Exception($"Error at Excel Row : {ex.Message}");
                    }
                
        }
       
    }
}