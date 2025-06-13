using AutoMapper;
using Contracts.Events.Maintenance;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IWorkOrder;
using Core.Domain.Common;
using Core.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, ApiResponseDTO<bool>>
    { 
        private readonly IWorkOrderCommandRepository _workOrderRepository;
        private readonly IWorkOrderQueryRepository _workOrderQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;         
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<UpdateWorkOrderCommandHandler> _logger;        
        private readonly ILogQueryService _logQueryService;
        private readonly IUnitGrpcClient _unitGrpcClient; 
        private readonly ICompanyGrpcClient _companyGrpcClient; 

        public UpdateWorkOrderCommandHandler(IWorkOrderCommandRepository workOrderRepository, IMapper mapper,IWorkOrderQueryRepository workOrderQueryRepository, IMediator mediator, IEventPublisher eventPublisher, ILogger<UpdateWorkOrderCommandHandler> logger, ILogQueryService logQueryService, IUnitGrpcClient unitGrpcClient,ICompanyGrpcClient companyGrpcClient) 
               {
            _workOrderRepository = workOrderRepository;
            _mapper = mapper;
            _workOrderQueryRepository = workOrderQueryRepository;
            _mediator = mediator;         
            _eventPublisher = eventPublisher;
            _logger = logger;         
            _logQueryService = logQueryService ?? throw new ArgumentNullException(nameof(logQueryService));  
             _unitGrpcClient = unitGrpcClient;
            _companyGrpcClient=companyGrpcClient;
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
               
                var miscMaster = await _workOrderRepository.GetMiscMasterByCodeAsync(MiscEnumEntity.MaintenanceStatusUpdate.Code);
                // Check if the code matches
              
                //var closedStatusId = miscMaster.Id;                
                if (updatedEntity.StatusId == miscMaster.Id  && updatedEntity.PreventiveScheduleId.HasValue)                
                {
                    var correlationId = Guid.NewGuid(); // ✅ Always create new correlationId
                    var @event = new WorkOrderClosedEvent
                    {
                        CorrelationId = correlationId,
                        WorkOrderId = updatedEntity.Id,
                        PreventiveSchedulerDetailId = updatedEntity.PreventiveScheduleId.Value
                    };
                    // Save and publish event (RabbitMQ/Saga)
                    await _eventPublisher.SaveEventAsync(@event);
                    await _eventPublisher.PublishPendingEventsAsync();
              
                    _logger.LogInformation("✅ WorkOrderClosedEvent published. CorrelationId: {CorrelationId}, WorkOrderId: {WorkOrderId}",
                        correlationId, updatedEntity.Id);
                }
                
                
                string tempFilePath = request.WorkOrder.Image;
                if (tempFilePath != null){
                    string baseDirectory = await _workOrderQueryRepository.GetBaseDirectoryAsync();
                       //GRPC
                    var units = await _unitGrpcClient.GetAllUnitAsync();
                    var companies = await _companyGrpcClient.GetAllCompanyAsync();
                    var unitLookup = units.ToDictionary(u => u.UnitId, u => u.UnitName);
                    var companyLookup = companies.ToDictionary(u => u.CompanyId, u => u.CompanyName);
                    string? unitName = null;
                    string? companyName = null;

                    if (unitLookup.TryGetValue(request.WorkOrder.UnitId, out var unitNameGrpc))
                    {
                        unitName= unitNameGrpc;
                    }
                    if (companyLookup.TryGetValue(request.WorkOrder.CompanyId, out var companyNameGrpc))
                    {
                        companyName= companyNameGrpc;
                    } 
                    //var (companyName, unitName) = await _workOrderRepository.GetCompanyUnitAsync(request.WorkOrder.CompanyId, request.WorkOrder.UnitId);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);     
                    //string companyFolder = Path.Combine(baseDirectory, companyName.Trim());
                    //string unitFolder = Path.Combine(companyFolder,unitName.Trim());
                     string filePath = Path.Combine(uploadPath, tempFilePath);  
                    EnsureDirectoryExists(Path.GetDirectoryName(filePath));           

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
                            Log.Error(ex, "Failed to rename file: {ErrorMessage}", ex.Message);
                        }
                    }
                }                    
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "WorkOrder updated."                     
                };
            }
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "WorkOrder not updated."
            };                
        }
           private void EnsureDirectoryExists(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }          
    }
 }
