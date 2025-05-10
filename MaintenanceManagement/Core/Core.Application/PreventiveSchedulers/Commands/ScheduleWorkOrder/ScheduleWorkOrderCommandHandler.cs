using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.Common.Interfaces.IWorkOrder;
using MediatR;
using static Core.Domain.Common.MiscEnumEntity;

namespace Core.Application.PreventiveSchedulers.Commands.ScheduleWorkOrder
{
    public class ScheduleWorkOrderCommandHandler : IRequestHandler<ScheduleWorkOrderCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        public ScheduleWorkOrderCommandHandler( IPreventiveSchedulerQuery preventiveSchedulerQuery,IMapper mapper, IMediator mediator,IMiscMasterQueryRepository miscMasterQueryRepository ,IWorkOrderCommandRepository workOrderRepository)
        {
            _preventiveSchedulerQuery=preventiveSchedulerQuery;
            _mapper=mapper;
            _mediator=mediator;
            _miscMasterQueryRepository=miscMasterQueryRepository;
            _workOrderRepository=workOrderRepository;
        }
        public async Task<ApiResponseDTO<bool>> Handle(ScheduleWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);
            var scheduledetail = await _preventiveSchedulerQuery.GetWorkOrderScheduleDetailById(request.PreventiveScheduleId);

             
                
                       var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(scheduledetail, opt =>
                        {
                            opt.Items["StatusId"] = miscdetail.Id;
                            opt.Items["PreventiveSchedulerDetailId"] = scheduledetail.PreventiveSchedulerDetails?.FirstOrDefault()?.Id;   
                        });

                        var response = await _workOrderRepository.CreateAsync(workOrderRequest,scheduledetail.MaintenanceCategoryId, cancellationToken);
                        if (response.Id == 0)
                        {
                             return new ApiResponseDTO<bool>
                              {
                                  IsSuccess = false, 
                                  Message = "Work Order not create."
                              };
                        }
                          return new ApiResponseDTO<bool>
                              {
                                  IsSuccess = true, 
                                  Message = "Work Order created."
                              };
             
        }
    }
}