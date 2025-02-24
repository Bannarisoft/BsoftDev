using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetLocation.Commands.CreateAssetLocation
{
    public class CreateAssetLocationCommandHandler  : IRequestHandler<CreateAssetLocationCommand, ApiResponseDTO<AssetLocationDto>>
    {
        private readonly IAssetLocationCommandRepository _assetLocationCommandRepository;
        private readonly IAssetLocationQueryRepository _assetLocationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateAssetLocationCommandHandler(
            IAssetLocationCommandRepository assetLocationCommandRepository,
            IMapper mapper,
            IMediator mediator,
            IAssetLocationQueryRepository assetLocationQueryRepository)
        {
            _assetLocationCommandRepository = assetLocationCommandRepository;
            _mapper = mapper;
            _mediator = mediator;
            _assetLocationQueryRepository = assetLocationQueryRepository;
        }

        public async Task<ApiResponseDTO<AssetLocationDto>> Handle(CreateAssetLocationCommand request, CancellationToken cancellationToken)
        {
            // Check if AssetLocation with the same AssetId already exists
            var existingAssetLocation = await _assetLocationQueryRepository.GetByIdAsync(request.AssetId);
            
            if (existingAssetLocation != null)
            {
                return new ApiResponseDTO<AssetLocationDto>
                {
                    IsSuccess = false,
                    Message = "Asset Location already exists",
                    Data = null
                };
            }

            // Map request to domain entity
            var assetLocation = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetLocation>(request);

            // Insert into the database
            var result = await _assetLocationCommandRepository.CreateAsync(assetLocation);
            if (result.Id <= 0)
            {
                return new ApiResponseDTO<AssetLocationDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Asset Location",
                    Data = null
                };
            }

            // Fetch newly created record
            var createdAssetLocation = await _assetLocationQueryRepository.GetByIdAsync(result.AssetId);
            var mappedResult = _mapper.Map<AssetLocationDto>(createdAssetLocation);

            // Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: mappedResult.AssetId.ToString(),
                actionName: mappedResult.LocationId.ToString(),
                details: $"Asset Location '{mappedResult.AssetId}' was created at Location: {mappedResult.LocationId}",
                module: "AssetLocation"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // Return success response
            return new ApiResponseDTO<AssetLocationDto>
            {
                IsSuccess = true,
                Message = "Asset Location created successfully",
                Data = mappedResult
            };
        }
    }

}