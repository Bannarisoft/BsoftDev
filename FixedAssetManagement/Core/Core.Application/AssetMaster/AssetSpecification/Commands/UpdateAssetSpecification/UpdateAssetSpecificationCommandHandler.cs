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
    public class UpdateAssetSpecificationCommandHandler : IRequestHandler<UpdateAssetSpecificationCommand, ApiResponseDTO<string>>
    {
        private readonly IAssetSpecificationCommandRepository _assetSpecificationRepository;
        private readonly IAssetSpecificationQueryRepository _assetSpecificationQueryRepository;
        private readonly IMediator _mediator;

        public UpdateAssetSpecificationCommandHandler(
            IAssetSpecificationCommandRepository assetSpecificationRepository,
            IAssetSpecificationQueryRepository assetSpecificationQueryRepository,
            IMediator mediator)
        {
            _assetSpecificationRepository = assetSpecificationRepository;
            _assetSpecificationQueryRepository = assetSpecificationQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<string>> Handle(UpdateAssetSpecificationCommand request, CancellationToken cancellationToken)
        {
            // Fetch existing specifications for the asset
            var existingSpecs = await _assetSpecificationQueryRepository.GetByIdAsync(request.AssetId);
            if (existingSpecs == null)
            {
                return new ApiResponseDTO<string>
                {
                    IsSuccess = false,
                    Message = "Asset not found or specifications are not available."
                };
            }

            int updateCount = 0;

            // Iterate through each specification to update
            foreach (var spec in request.Specifications)
            {
                var exists = await _assetSpecificationRepository.ExistsByAssetSpecIdAsync(request.AssetId, spec.SpecificationId);
                if (exists)
                {
                    // Prepare the updated specification
                    var updatedSpec = new AssetSpecifications
                    {
                        AssetId = request.AssetId,
                        SpecificationId = spec.SpecificationId,
                        SpecificationValue = spec.SpecificationValue,
                        // Ensure correct mapping for IsActive field
                        IsActive = spec.IsActive == 1 ? BaseEntity.Status.Active : BaseEntity.Status.Inactive
                    };

                    // Update the specification in the repository
                    await _assetSpecificationRepository.UpdateAsync(request.AssetId, updatedSpec);
                    updateCount++;

                    // Publish domain event for the update
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: request.AssetId.ToString(),
                        actionName: spec.SpecificationId.ToString(),
                        details: $"Updated specification '{spec.SpecificationId}' to value '{spec.SpecificationValue}'",
                        module: "AssetSpecification"
                    );

                    await _mediator.Publish(domainEvent, cancellationToken);
                }
            }

            // Return a response based on whether any updates were made
            return new ApiResponseDTO<string>
            {
                IsSuccess = updateCount > 0,
                Message = updateCount > 0 ? "Specifications updated successfully." : "No specifications updated."
            };
        }
    }
}
