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
    public class UpdateAssetWarrantyCommandHandler : IRequestHandler<UpdateAssetWarrantyCommand, ApiResponseDTO<AssetWarrantyDTO>>
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

        public async Task<ApiResponseDTO<AssetWarrantyDTO>> Handle(UpdateAssetWarrantyCommand request, CancellationToken cancellationToken)
        {
            var assetWarranty = await _assetWarrantyQueryRepository.GetByIdAsync(request.Id);
            if (assetWarranty is null)
            return new ApiResponseDTO<AssetWarrantyDTO>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist or is inactive."
            };
           
            var oldAssetWarranty= assetWarranty.Id;
            assetWarranty.Id = request.Id;

            if (assetWarranty is null || assetWarranty.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid Warranty Id. The specified Id does not exist or is deleted."
                };
            }
            if (assetWarranty.IsActive != request.IsActive)
            {    
                assetWarranty.IsActive =  (BaseEntity.Status)request.IsActive;     
                var updatedAssetWarranty = _mapper.Map<AssetWarranties>(request);                   
                await _assetWarrantyRepository.UpdateAsync(request.Id, updatedAssetWarranty);
        
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<AssetWarrantyDTO>
                    {
                        IsSuccess = true,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<AssetWarrantyDTO>
                    {
                        IsSuccess = true,
                        Message = "Code Activated."
                    }; 
                }                                     
            }

            var assetWarrantyExistsByName = await _assetWarrantyRepository.ExistsByAssetIdAsync(request.AssetId);
            if (assetWarrantyExistsByName)
            {                                   
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = false,
                    Message = $"Asset Warranty already exists and is {(BaseEntity.Status) request.IsActive}."
                };                     
            }
            var updatedAssetSpecEntity = _mapper.Map<AssetWarranties>(request);                   
            var updateResult = await _assetWarrantyRepository.UpdateAsync(request.Id, updatedAssetSpecEntity);            

            var updatedAssetSpec =  await _assetWarrantyQueryRepository.GetByIdAsync(request.Id);    
            if (updatedAssetSpec != null)
            {
                var AssetSpecDto = _mapper.Map<AssetWarrantyDTO>(updatedAssetSpec);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: AssetSpecDto.AssetId.ToString(),
                    actionName: AssetSpecDto.WarrantyType.ToString() ?? string.Empty,
                    details: $"AssetWarranty '{oldAssetWarranty}' was updated to '{AssetSpecDto.Description}'",
                    module:"AssetWarranty"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<AssetWarrantyDTO>
                    {
                        IsSuccess = true,
                        Message = "AssetWarranty updated successfully.",
                        Data = AssetSpecDto
                    };
                }
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = false,
                    Message = "AssetWarranty not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<AssetWarrantyDTO>{
                    IsSuccess = false,
                    Message = "AssetWarranty not found."
                };
            }
        }
    }
}