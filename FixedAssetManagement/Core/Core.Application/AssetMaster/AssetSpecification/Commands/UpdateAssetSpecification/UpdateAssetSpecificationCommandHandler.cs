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
    public class UpdateAssetSpecificationCommandHandler  : IRequestHandler<UpdateAssetSpecificationCommand, ApiResponseDTO<AssetSpecificationJsonDto>>
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

        public async Task<ApiResponseDTO<AssetSpecificationJsonDto>> Handle(UpdateAssetSpecificationCommand request, CancellationToken cancellationToken)
    {
        var assetSpecifications = await _assetSpecificationQueryRepository.GetByIdAsync(request.Id);
        
        if (assetSpecifications is null)
        {
            return new ApiResponseDTO<AssetSpecificationJsonDto>
            {
                IsSuccess = false,
                Message = "Invalid DepreciationGroupID. The specified Name does not exist or is inactive."
            };
        }

        // ✅ Get first specification safely
        var oldAssetSpecification = assetSpecifications.Specifications.FirstOrDefault()?.SpecificationId;

        // ✅ Update the first specification if exists
        if (assetSpecifications.Specifications.Any())
        {
            assetSpecifications.Specifications[0].SpecificationId = request.SpecificationId;
        }

        if (assetSpecifications.IsActive != request.IsActive)
        {    
            assetSpecifications.IsActive = (BaseEntity.Status)request.IsActive;     
            var updatedAssetSpecification = _mapper.Map<AssetSpecifications>(request);                   
            await _assetSpecificationRepository.UpdateAsync(request.Id, updatedAssetSpecification);

            return new ApiResponseDTO<AssetSpecificationJsonDto>
            {
                IsSuccess = true,
                Message = request.IsActive == 0 ? "Code DeActivated." : "Asset Specification Details updated."                
            };
        }

        var assetSpecificationExistsByName = await _assetSpecificationRepository.ExistsByAssetSpecIdAsync(request.AssetId, request.SpecificationId);
        if (assetSpecificationExistsByName)
        {                                   
            return new ApiResponseDTO<AssetSpecificationJsonDto>
            {
                IsSuccess = false,
                Message = $"Asset Specification already exists and is {(BaseEntity.Status)request.IsActive}."
            };                     
        }

        var updatedAssetSpecEntity = _mapper.Map<AssetSpecifications>(request);                   
        var updateResult = await _assetSpecificationRepository.UpdateAsync(request.Id, updatedAssetSpecEntity);            

        var updatedAssetSpec = await _assetSpecificationQueryRepository.GetByIdAsync(request.Id);    

        if (updatedAssetSpec != null)
        {
            var AssetSpecDto = _mapper.Map<AssetSpecificationJsonDto>(updatedAssetSpec);
            
            // ✅ Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: AssetSpecDto.AssetId.ToString(),
                actionName: oldAssetSpecification?.ToString() ?? "N/A",
                details: $"AssetSpecification '{oldAssetSpecification}' was updated to '{AssetSpecDto.Specifications.FirstOrDefault()?.SpecificationValue}'. Code: {request.SpecificationId}",
                module: "AssetSpecification"
            );            
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<AssetSpecificationJsonDto>
            {
                IsSuccess = true,
                Message = "AssetSpecification updated successfully.",
                Data = AssetSpecDto
            };
        }

        return new ApiResponseDTO<AssetSpecificationJsonDto>
        {
            IsSuccess = false,
            Message = "AssetSpecification not updated."
        };
    }

    }
}