
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrderMaster.IWorkOrder;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Events;
using MassTransit.Mediator;
using MediatR;

namespace Core.Application.WorkOrderMaster.WorkOrder.Command.CreateWorkOrder
{
   /*  public class CreateWorkOrderCommandHandler : IRequestHandler<CreateWorkOrderCommand, ApiResponseDTO<WorkOrderCombineDto>>
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
            var assetEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request.AssetMaster);            
            var result = await _workOrderRepository.CreateAsync(assetEntity, cancellationToken);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetCode ?? string.Empty,
                actionName: assetEntity.AssetName ?? string.Empty,
                details: $"AssetMasterGeneral '{assetEntity.AssetName}' was created. Code: {assetEntity.AssetCode}",
                module: "AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
        
            var assetMasterDTO = _mapper.Map<WorkOrderCombineDto>(result);
            if (result.Id > 0)
            {
                string tempFilePath = request.AssetMaster.AssetImage;
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    string directory = Path.GetDirectoryName(tempFilePath) ?? string.Empty;
                    string newFileName = $"{result.AssetCode}{Path.GetExtension(tempFilePath)}";
                    string newFilePath = Path.Combine(directory, newFileName);

                    try
                    {
                        File.Move(tempFilePath, newFilePath);
                        assetEntity.AssetImage = newFilePath.Replace(@"\", "/");
                        await _assetMasterGeneralRepository.UpdateAsync(assetEntity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to rename file: {ex.Message}");
                    }
                }
                return new ApiResponseDTO<WorkOrderCombineDto>
                {
                    IsSuccess = true,
                    Message = "AssetMasterGeneral created successfully.",
                    Data = assetMasterDTO
                };
            }
            return new ApiResponseDTO<WorkOrderCombineDto>
            {
                IsSuccess = false,
                Message = "AssetMasterGeneral not created."
            };
        }
    } */
}