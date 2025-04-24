using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, ApiResponseDTO<bool>>
    { 
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;         

        public UpdateWorkOrderCommandHandler(IWorkOrderCommandRepository workOrderRepository, IMapper mapper,IWorkOrderQueryRepository workOrderQueryRepository, IMediator mediator)
        {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _workOrderQueryRepository = workOrderQueryRepository;
            _mediator = mediator;            
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
        {
            
            var updatedEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request.WorkOrder);
            var updateResult = await _workOrderRepository.UpdateAsync(updatedEntity.Id, updatedEntity);

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: updatedEntity.WorkOrderDocNo ?? string.Empty,
                actionName: "WorkOrder Update",
                details: $"WorkOrder updated for ID {updatedEntity.Id}",
                module: "WorkOrder"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            if(updateResult)
            {
                string tempFilePath = request.WorkOrder.Image;
                if (tempFilePath != null){
                    string baseDirectory = await _workOrderQueryRepository.GetBaseDirectoryAsync();

                    var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(request.WorkOrder.CompanyId, request.WorkOrder.UnitId);

                    string companyFolder = Path.Combine(baseDirectory, companyName.Trim());
                    string unitFolder = Path.Combine(companyFolder,unitName.Trim());
                    string filePath = Path.Combine(unitFolder, tempFilePath);                

                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                        string newFileName = $"{request.WorkOrder.WorkOrderDocNo}{Path.GetExtension(tempFilePath)}";
                        string newFilePath = Path.Combine(directory, newFileName);

                        try
                        {
                            File.Move(filePath, newFilePath);
                            //assetEntity.AssetImage = newFileName;
                            await _workOrderRepository.UpdateWOImageAsync(request.WorkOrder.Id, newFileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to rename file: {ex.Message}");
                        }
                    }
                }                    
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "WorkOrder updated successfully.",                        
                };
            }
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "WorkOrder not updated."
            };                
        }          
    }
 }
