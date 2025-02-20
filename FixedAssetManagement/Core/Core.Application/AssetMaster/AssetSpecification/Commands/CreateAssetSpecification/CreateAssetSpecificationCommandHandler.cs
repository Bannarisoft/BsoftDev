using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification
{
    public class CreateAssetSpecificationCommandHandler : IRequestHandler<CreateAssetSpecificationCommand, ApiResponseDTO<AssetSpecificationDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetSpecificationCommandRepository _assetSpecificationRepository;
        private readonly IMediator _mediator;

        public CreateAssetSpecificationCommandHandler(IMapper mapper, IAssetSpecificationCommandRepository assetSpecificationRepository, IMediator mediator)
        {
            _mapper = mapper;
            _assetSpecificationRepository = assetSpecificationRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<AssetSpecificationDTO>> Handle(CreateAssetSpecificationCommand request, CancellationToken cancellationToken)
        {
            var assetSpecificationExists = await _assetSpecificationRepository.ExistsByAssetSpecIdAsync(request.AssetId, request.SpecificationId);
            if (assetSpecificationExists)
            {
                return new ApiResponseDTO<AssetSpecificationDTO> {
                    IsSuccess = false, 
                    Message = "Asset Specification already exists."
                };                 
            }
            var assetEntity = _mapper.Map<AssetSpecifications>(request);     
            var result = await _assetSpecificationRepository.CreateAsync(assetEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetEntity.AssetId.ToString() ?? string.Empty,
                actionName: assetEntity.SpecificationId.ToString() ?? string.Empty,
                details: $"Asset Specification '{assetEntity.SpecificationValue}' was created.Value {assetEntity.SpecificationValue}",
                module:"Asset Specification"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var assetMasterDTO = _mapper.Map<AssetSpecificationDTO>(result);
            if (assetMasterDTO.Id > 0)
            {
                return new ApiResponseDTO<AssetSpecificationDTO>{
                    IsSuccess = true, 
                    Message = "Asset Specification created successfully.",
                    Data = assetMasterDTO
                };
            }
            return  new ApiResponseDTO<AssetSpecificationDTO>{
                IsSuccess = false, 
                Message = "Asset Specification not created."
            };      
        }
    }
}