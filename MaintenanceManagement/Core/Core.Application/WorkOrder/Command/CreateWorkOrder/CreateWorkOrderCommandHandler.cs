

using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, ApiResponseDTO<WorkOrderCombineDto>>
    {
        private readonly IMapper _mapper;
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMediator _mediator;
        private readonly IIPAddressService _ipAddressService;

        public CreateWorkOrderCommandHandler(IMapper mapper, IWorkOrderCommandRepository workOrderRepository, IWorkOrderQueryRepository workOrderQueryRepository, IMediator mediator, IIPAddressService ipAddressService)
        {
            _mapper = mapper;
            _workOrderRepository = workOrderRepository;
            _workOrderQueryRepository = workOrderQueryRepository;
            _mediator = mediator;     
            _ipAddressService = ipAddressService;    
        }

        public async Task<ApiResponseDTO<WorkOrderCombineDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var companyId = _ipAddressService.GetCompanyId();
            var unitId = _ipAddressService.GetUnitId();
            var latestWoCode = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(request.WorkOrderDto.RequestTypeId);            
            var woEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request.WorkOrderDto);   
            woEntity.WorkOrderDocNo = latestWoCode;         
            woEntity.CompanyId = companyId; 
            woEntity.UnitId = unitId; 
            woEntity.TotalManPower=0;
            woEntity.TotalSpentHours=0;            
            var result = await _workOrderRepository.CreateAsync(woEntity, cancellationToken);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: "",
                actionName: woEntity.WorkOrderDocNo??string.Empty,
                details: $"WorkOrder '{latestWoCode}' was created",
                module: "WorkOrder"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
        
            var woMasterDTO = _mapper.Map<WorkOrderCombineDto>(result);
            if (result.Id > 0)
            {                
                return new ApiResponseDTO<WorkOrderCombineDto>
                {
                    IsSuccess = true,
                    Message = "Work Order created successfully.",
                    Data = woMasterDTO
                };
            }
            return new ApiResponseDTO<WorkOrderCombineDto>
            {
                IsSuccess = false,
                Message = "Work Order not created."
            };
        }
    } 
}