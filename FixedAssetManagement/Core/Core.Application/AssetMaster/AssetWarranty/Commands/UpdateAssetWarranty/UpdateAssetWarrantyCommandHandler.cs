using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.UpdateAssetWarranty
{
    public class UpdateAssetWarrantyCommandHandler : IRequestHandler<UpdateAssetWarrantyCommand, ApiResponseDTO<bool>>
    {
        private readonly IAssetWarrantyCommandRepository _assetWarrantyRepository;
        private readonly IAssetWarrantyQueryRepository _assetWarrantyQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateAssetWarrantyCommandHandler(IAssetWarrantyCommandRepository assetWarrantyRepository, IMapper mapper,IAssetWarrantyQueryRepository assetWarrantyQueryRepository, IMediator mediator)
        {
            _assetWarrantyRepository = assetWarrantyRepository;
            _mapper = mapper;
            _assetWarrantyQueryRepository = assetWarrantyQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            var assetWarranty = await _assetWarrantyQueryRepository.GetByIdAsync(request.Id);
            if (assetWarranty is null)
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist "
            };
           
            var oldAssetWarranty= assetWarranty.Id;            
        
           
            var updatedAssetSpecEntity = _mapper.Map<AssetWarranties>(request);                   
            var updateResult = await _assetWarrantyRepository.UpdateAsync(updatedAssetSpecEntity);            

                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.AssetId.ToString(),
                    actionName: request.WarrantyType.ToString() ?? string.Empty,
                    details: $"AssetWarranty '{oldAssetWarranty}' was updated to '{request.Description}'",
                    module:"AssetWarranty"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult)
                {
                    
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Asset Warranty updated successfully."};
                }
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "AssetWarranty not updated."
                };                
            }           
    }
}