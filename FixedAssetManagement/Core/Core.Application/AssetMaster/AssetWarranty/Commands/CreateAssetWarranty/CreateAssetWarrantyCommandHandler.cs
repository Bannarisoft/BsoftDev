using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.CreateAssetWarranty
{
    public class CreateAssetWarrantyCommandHandler : IRequestHandler<CreateAssetWarrantyCommand, ApiResponseDTO<AssetWarrantyDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly IMediator _mediator;

        public CreateAssetWarrantyCommandHandler(IMapper mapper, IAssetWarrantyCommandRepository assetWarrantyRepository, IMediator mediator)
        {
            _mapper = mapper;
            _assetWarrantyRepository = assetWarrantyRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<AssetWarrantyDTO>> Handle(CreateAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            var assetSpecificationExists = await _assetWarrantyRepository.ExistsByAssetIdAsync(request.AssetId);
            if (assetSpecificationExists)
            {
                return new ApiResponseDTO<AssetWarrantyDTO> {
                    IsSuccess = false, 
                    Message = "Asset Warranty already exists."
                };                 
            }
            var assetEntity = _mapper.Map<AssetWarranties>(request);     
            var result = await _assetWarrantyRepository.CreateAsync(assetEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetId.ToString() ?? string.Empty,
                actionName: assetEntity.WarrantyType.ToString() ?? string.Empty,
                details: $"Asset Warranty '{assetEntity.Description}' was created.",
                module:"Asset Warranty"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var assetMasterDTO = _mapper.Map<AssetWarrantyDTO>(result);
            if (assetMasterDTO.Id > 0)
            {
                return new ApiResponseDTO<AssetWarrantyDTO>{
                    IsSuccess = true, 
                    Message = "Asset Warranty created successfully.",
                    Data = assetMasterDTO
                };
            }
            return  new ApiResponseDTO<AssetWarrantyDTO>{
                IsSuccess = false, 
                Message = "Asset Warranty not created."
            };      
        }
    }
}