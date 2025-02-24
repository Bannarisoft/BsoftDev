using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification
{
    public class UpdateAssetSpecificationCommandHandler  : IRequestHandler<UpdateAssetSpecificationCommand, ApiResponseDTO<AssetSpecificationDTO>>
    {
        private readonly IAssetSpecificationCommandRepository _assetSpecificationRepository;
        private readonly IAssetSpecificationQueryRepository _assetSpecificationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateAssetSpecificationCommandHandler(IAssetSpecificationCommandRepository assetSpecificationRepository, IMapper mapper,IAssetSpecificationQueryRepository assetSpecificationQueryRepository, IMediator mediator)
        {
            _assetSpecificationRepository = assetSpecificationRepository;
            _mapper = mapper;
            _assetSpecificationQueryRepository = assetSpecificationQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetSpecificationDTO>> Handle(UpdateAssetSpecificationCommand request, CancellationToken cancellationToken)
        {
            var assetSpecifications = await _assetSpecificationQueryRepository.GetByIdAsync(request.Id);
            if (assetSpecifications is null)
            return new ApiResponseDTO<AssetSpecificationDTO>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist or is inactive."
            };
           
            var oldAssetSpecification= assetSpecifications.SpecificationId;
            assetSpecifications.SpecificationId = request.SpecificationId;

            if (assetSpecifications is null || assetSpecifications.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid Specification Id. The specified Id does not exist or is deleted."
                };
            }
            if (assetSpecifications.IsActive != request.IsActive)
            {    
                assetSpecifications.IsActive =  (BaseEntity.Status)request.IsActive;     
                var updatedAssetSpecification = _mapper.Map<AssetSpecifications>(request);                   
                await _assetSpecificationRepository.UpdateAsync(request.Id, updatedAssetSpecification);
        
                //await _assetSpecificationRepository.UpdateAsync(assetSpecifications.Id, assetSpecifications);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<AssetSpecificationDTO>
                    {
                        IsSuccess = true,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<AssetSpecificationDTO>
                    {
                        IsSuccess = true,
                        Message = "Code Activated."
                    }; 
                }                                     
            }

            var assetSpecificationExistsByName = await _assetSpecificationRepository.ExistsByAssetSpecIdAsync(request.AssetId, request.SpecificationId);
            if (assetSpecificationExistsByName)
            {                                   
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = false,
                    Message = $"Asset Specification already exists and is {(BaseEntity.Status) request.IsActive}."
                };                     
            }
            var updatedAssetSpecEntity = _mapper.Map<AssetSpecifications>(request);                   
            var updateResult = await _assetSpecificationRepository.UpdateAsync(request.Id, updatedAssetSpecEntity);            

            var updatedAssetSpec =  await _assetSpecificationQueryRepository.GetByIdAsync(request.Id);    
            if (updatedAssetSpec != null)
            {
                var AssetSpecDto = _mapper.Map<AssetSpecificationDTO>(updatedAssetSpec);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: AssetSpecDto.AssetId.ToString(),
                    actionName: AssetSpecDto.SpecificationId.ToString(),
                    details: $"AssetSpecification '{oldAssetSpecification}' was updated to '{AssetSpecDto.SpecificationValue}'.  Code: {AssetSpecDto.SpecificationId}",
                    module:"AssetSpecification"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<AssetSpecificationDTO>
                    {
                        IsSuccess = true,
                        Message = "AssetSpecification updated successfully.",
                        Data = AssetSpecDto
                    };
                }
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = false,
                    Message = "AssetSpecification not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<AssetSpecificationDTO>{
                    IsSuccess = false,
                    Message = "AssetSpecification not found."
                };
            }
        }
    }
}