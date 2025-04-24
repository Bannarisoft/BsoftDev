

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
            var companyId = 1;//_ipAddressService.GetCompanyId();
            var unitId = 41;//_ipAddressService.GetUnitId();
            //var latestWoCode = await _workOrderQueryRepository.GetLatestWorkOrderDocNo(request.WorkOrderDto.RequestTypeId);            
            var woEntity = _mapper.Map<Core.Domain.Entities.WorkOrderMaster.WorkOrder>(request.WorkOrderDto);   
            //woEntity.WorkOrderDocNo = latestWoCode;         
            woEntity.CompanyId = companyId; 
            woEntity.UnitId = unitId; 
            woEntity.TotalManPower=0;
            woEntity.TotalSpentHours=0;    
            woEntity.CreatedByName=     _ipAddressService.GetUserName();   
            woEntity.CreatedBy=      int.Parse(_ipAddressService.GetCurrentUserId());
            woEntity.CreatedIP=     _ipAddressService.GetSystemIPAddress();
            var result = await _workOrderRepository.CreateAsync(woEntity,request.WorkOrderDto.RequestTypeId, cancellationToken);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: "",
                actionName: woEntity.WorkOrderDocNo??string.Empty,
                details: $"WorkOrder '{woEntity.WorkOrderDocNo}' was created",
                module: "WorkOrder"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
        
            var woMasterDTO = _mapper.Map<WorkOrderCombineDto>(result);
            if (result.Id > 0)
            {           
                string tempFilePath = request.WorkOrderDto.Image;
                if (tempFilePath != null){
                    string baseDirectory = await _workOrderQueryRepository.GetBaseDirectoryAsync();

                    var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(companyId, unitId);

                    string companyFolder = Path.Combine(baseDirectory, companyName.Trim());
                    string unitFolder = Path.Combine(companyFolder,unitName.Trim());
                    string filePath = Path.Combine(unitFolder, tempFilePath);

            

                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                        string newFileName = $"{woEntity.WorkOrderDocNo}{Path.GetExtension(tempFilePath)}";
                        string newFilePath = Path.Combine(directory, newFileName);

                        try
                        {
                            File.Move(filePath, newFilePath);
                            //assetEntity.AssetImage = newFileName;
                            await _workOrderRepository.UpdateWOImageAsync(woEntity.Id, newFileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to rename file: {ex.Message}");
                        }
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