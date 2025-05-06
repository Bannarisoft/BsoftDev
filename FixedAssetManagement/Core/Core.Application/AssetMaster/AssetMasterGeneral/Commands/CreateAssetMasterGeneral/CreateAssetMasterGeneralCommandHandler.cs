using AutoMapper;
using Contracts.Events.Users;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral
{
    public class CreateAssetMasterGeneralCommandHandler : IRequestHandler<CreateAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterDto>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;  // Use IEventPublisher instead of IPublishEndpoint

        public CreateAssetMasterGeneralCommandHandler(IMapper mapper, IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository, IMediator mediator, IEventPublisher eventPublisher)
        {
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
        }

        public async Task<ApiResponseDTO<AssetMasterDto>> Handle(CreateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            // Get latest AssetCode
            var latestAssetCode = await _assetMasterGeneralQueryRepository.GetLatestAssetCode( request.AssetMaster.AssetGroupId, request.AssetMaster.AssetCategoryId, request.AssetMaster.AssetLocation.DepartmentId, request.AssetMaster.AssetLocation.LocationId);
            var assetCode = latestAssetCode;
            var assetEntity = _mapper.Map<AssetMasterGenerals>(request.AssetMaster);
            assetEntity.AssetCode = assetCode;
            var result = await _assetMasterGeneralRepository.CreateAsync(assetEntity, cancellationToken);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetCode ?? string.Empty,
                actionName: assetEntity.AssetName ?? string.Empty,
                details: $"AssetMasterGeneral '{assetEntity.AssetName}' was created. Code: {assetEntity.AssetCode}",
                module: "AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);


            // Use the ID generated from the database
            var assetId = result.Id;
            var assetCreatedEvent = new AssetCreatedEvent
            {
                CorrelationId = Guid.NewGuid(),
                AssetId = assetId,
                AssetName = assetEntity.AssetName
                // UserId = assetEntity.UserId
            };

            // Save event to Outbox 
            await _eventPublisher.SaveEventAsync(assetCreatedEvent);

            // Triggering the publishing of pending events
            await _eventPublisher.PublishPendingEventsAsync();


            var assetMasterDTO = _mapper.Map<AssetMasterDto>(result);
            if (result.Id > 0)
            {
             
                string tempFilePath = request.AssetMaster.AssetImage;
                if (tempFilePath != null){
                    string baseDirectory = await _assetMasterGeneralQueryRepository.GetBaseDirectoryAsync();
                    var (companyName, unitName) = await _assetMasterGeneralQueryRepository.GetCompanyUnitAsync(request.AssetMaster.CompanyId ,request.AssetMaster.UnitId);
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", baseDirectory,companyName,unitName);   
                    string filePath = Path.Combine(uploadPath, tempFilePath);
                    EnsureDirectoryExists(Path.GetDirectoryName(filePath));             

                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                        string newFileName = $"{result.AssetCode}{Path.GetExtension(tempFilePath)}";
                        string newFilePath = Path.Combine(directory, newFileName);

                        try
                        {
                            File.Move(filePath, newFilePath);
                            //assetEntity.AssetImage = newFileName;
                            await _assetMasterGeneralRepository.UpdateAssetImageAsync(assetEntity.Id, newFileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to rename file: {ex.Message}");
                        }
                    }
                }
                return new ApiResponseDTO<AssetMasterDto>
                {
                    IsSuccess = true,
                    Message = "AssetMasterGeneral created successfully.",
                    Data = assetMasterDTO
                };
            }
            return new ApiResponseDTO<AssetMasterDto>
            {
                IsSuccess = false,
                Message = "AssetMasterGeneral not created."
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