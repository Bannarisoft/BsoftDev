

using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
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

        public CreateWorkOrderCommandHandler(IMapper mapper, IWorkOrderCommandRepository workOrderRepository, IWorkOrderQueryRepository workOrderQueryRepository, IMediator mediator)
        {
            _mapper = mapper;
            _workOrderRepository = workOrderRepository;
            _workOrderQueryRepository = workOrderQueryRepository;
            _mediator = mediator;            
        }

        public async Task<ApiResponseDTO<WorkOrderCombineDto>> Handle(CreateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            var woEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request.WorkOrder);            
            var result = await _workOrderRepository.CreateAsync(woEntity, cancellationToken);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: "",
                actionName: woEntity.RequestId ?? string.Empty,
                details: $"WorkOrder '{woEntity.RequestId}' was created",
                module: "WorkOrder"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
        
            var woMasterDTO = _mapper.Map<WorkOrderCombineDto>(result);
            if (result.Id > 0)
            {
                string tempFilePath = request.WorkOrder.Image;
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    string directory = Path.GetDirectoryName(tempFilePath) ?? string.Empty;
                    string newFileName = $"{result.RequestId}{Path.GetExtension(tempFilePath)}";
                    string newFilePath = Path.Combine(directory, newFileName);

                    try
                    {
                        File.Move(tempFilePath, newFilePath);
                        woEntity.Image = newFilePath.Replace(@"\", "/");
                        await _workOrderRepository.UpdateAsync(woEntity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to rename file: {ex.Message}");
                    }
                }
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