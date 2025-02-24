using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteAssetWarranty
{
    public class DeleteAssetWarrantyCommandHandler : IRequestHandler<DeleteAssetWarrantyCommand, ApiResponseDTO<AssetWarrantyDTO>>
    {
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IAssetWarrantyQueryRepository _assetWarrantyQueryRepository;
        
        public DeleteAssetWarrantyCommandHandler(IAssetWarrantyCommandRepository assetWarrantyRepository, IMapper mapper,  IMediator mediator,IAssetWarrantyQueryRepository assetWarrantyQueryRepository)
        {
            _assetWarrantyRepository = assetWarrantyRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _assetWarrantyQueryRepository=assetWarrantyQueryRepository;
        }

        public async Task<ApiResponseDTO<AssetWarrantyDTO>> Handle(DeleteAssetWarrantyCommand request, CancellationToken cancellationToken)
        {             
            var assetWarranty = await _assetWarrantyQueryRepository.GetByIdAsync(request.Id);
            if (assetWarranty is null )
            {
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid DepreciationGroupID."
                };
            }
            var assetWarrantyDelete = _mapper.Map<AssetWarranties>(request);      
            var updateResult = await _assetWarrantyRepository.DeleteAsync(request.Id, assetWarrantyDelete);
            if (updateResult > 0)
            {
                var assetWarrantyDto = _mapper.Map<AssetWarrantyDTO>(assetWarrantyDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: assetWarrantyDelete.AssetId.ToString() ?? string.Empty,
                    actionName: assetWarrantyDelete.WarrantyType.ToString() ?? string.Empty,
                    details: $"AssetWarranty '{assetWarrantyDto.Description}' was created.",
                    module:"AssetWarranty"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = true,
                    Message = "Asset Warranty deleted successfully.",
                    Data = assetWarrantyDto
                };
            }
            return new ApiResponseDTO<AssetWarrantyDTO>
            {
                IsSuccess = false,
                Message = "Asset Warranty deletion failed."                             
            };           
        }
    }
}