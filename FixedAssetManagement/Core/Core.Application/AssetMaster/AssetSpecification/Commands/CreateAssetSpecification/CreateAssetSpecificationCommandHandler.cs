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
    public class CreateAssetSpecificationCommandHandler : IRequestHandler<CreateAssetSpecificationCommand, ApiResponseDTO<string>>
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

        public async Task<ApiResponseDTO<string>> Handle(CreateAssetSpecificationCommand request, CancellationToken cancellationToken)
        {
             var createdCount = 0;

            foreach (var spec in request.Specifications )
            {
                var alreadyExists = await _assetSpecificationRepository.ExistsByAssetSpecIdAsync(request.AssetId, spec.SpecificationId);
                if (!alreadyExists)
                {
                    var assetSpecification = new AssetSpecifications
                    {
                        AssetId = request.AssetId,
                        SpecificationId = spec.SpecificationId,
                        SpecificationValue = spec.SpecificationValue                                                                        
                    };

                    await _assetSpecificationRepository.CreateAsync(assetSpecification);
                    createdCount++;

                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Create",
                        actionCode: spec.SpecificationId.ToString() ?? string.Empty,
                        actionName: spec.SpecificationName ?? string.Empty,
                        details: $"Asset Specification '{spec.SpecificationValue}' created ",
                        module: "Asset Specification"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);
                }
                else{
                     return new ApiResponseDTO<string>
                    {
                        IsSuccess =false,
                        Message =  "Already Exists"
                    };  
                }
            }
            return new ApiResponseDTO<string>
            {
                IsSuccess = createdCount > 0,
                Message = createdCount > 0 ? "Specifications saved successfully." : "No new specifications were saved."
            };  
        }
    }
}