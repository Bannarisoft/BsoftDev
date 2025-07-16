using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
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
        private readonly IIPAddressService _ipAddressService;
        public ScheduleWorkOrderCommandHandler( IPreventiveSchedulerQuery preventiveSchedulerQuery,IMapper mapper, IMediator mediator,IMiscMasterQueryRepository miscMasterQueryRepository ,IWorkOrderCommandRepository workOrderRepository,IIPAddressService iPAddressService)
        {
            _preventiveSchedulerQuery=preventiveSchedulerQuery;
            _mapper=mapper;
            _mediator=mediator;
            _miscMasterQueryRepository=miscMasterQueryRepository;
            _workOrderRepository=workOrderRepository;
            _ipAddressService = iPAddressService;
        }
        public async Task<ApiResponseDTO<bool>> Handle(ScheduleWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var miscdetail = await _miscMasterQueryRepository.GetMiscMasterByName(WOStatus.MiscCode,StatusOpen.Code);
         //   var ExistItems = await _preventiveSchedulerQuery.ExistPreventivescheduleItem(request.PreventiveScheduleId);
           // var scheduledetail;
           
                 var scheduledetail = await _preventiveSchedulerQuery.GetWorkOrderScheduleDetailById(request.PreventiveScheduleId);
            
            

             await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: $"Schedule Work Order request",
                     actionCode: "Schedule work order",
                     actionName: "Schedule work order",
                     module: "Preventive",
                     requestData: request,
                     cancellationToken
                    );
                
                       var workOrderRequest =  _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(scheduledetail, opt =>
                        {
                            opt.Items["StatusId"] = miscdetail.Id;
                            opt.Items["PreventiveSchedulerDetailId"] = scheduledetail.PreventiveSchedulerDetails?.FirstOrDefault()?.Id;   
                        });
                        workOrderRequest.CreatedByName="System";
                        workOrderRequest.CreatedBy=1;
                        workOrderRequest.CreatedDate=DateTime.Now;
                        workOrderRequest.CreatedIP="192.168";
                        
                        await AuditLogPublisher.PublishAuditLogAsync(
                     _mediator,
                     actionDetail: $"Schedule Work Order creation request MaintenanceCategoryId:{scheduledetail.MaintenanceCategoryId}CompanyId:{scheduledetail.CompanyId}UnitId:{scheduledetail.UnitId}",
                     actionCode: "Schedule work order",
                     actionName: "Schedule work order",
                     module: "Preventive",
                     requestData: workOrderRequest,
                     cancellationToken
                    );

                        var response = await _workOrderRepository.CreatePreventiveAsync(workOrderRequest,scheduledetail.MaintenanceCategoryId,scheduledetail.CompanyId,scheduledetail.UnitId, cancellationToken);
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