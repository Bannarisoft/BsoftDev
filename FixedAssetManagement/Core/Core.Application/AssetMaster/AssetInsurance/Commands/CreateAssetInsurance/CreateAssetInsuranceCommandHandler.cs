using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetInsurance;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetInsurance.Commands.CreateAssetInsurance
{
    public class CreateAssetInsuranceCommandHandler : IRequestHandler<CreateAssetInsuranceCommand, ApiResponseDTO<GetAssetInsuranceDto>>
    {


         private readonly IAssetInsuranceCommandRepository _assetInsuranceCommandRepository;
        private readonly IAssetInsuranceQueryRepository _assetInsuranceQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

         public CreateAssetInsuranceCommandHandler(
            IAssetInsuranceCommandRepository assetInsuranceCommandRepository,
            IMapper mapper,
            IMediator mediator,
            IAssetInsuranceQueryRepository assetInsuranceQueryRepository)
        {
            _assetInsuranceCommandRepository = assetInsuranceCommandRepository;
            _mapper = mapper;
            _mediator = mediator;
            _assetInsuranceQueryRepository = assetInsuranceQueryRepository;
        }

        
        public async Task<ApiResponseDTO<GetAssetInsuranceDto>> Handle(CreateAssetInsuranceCommand request, CancellationToken cancellationToken)
        {

            // Map request to domain entity
            var assetInsurance = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetInsurance>(request);

            // Insert into the database
            var result = await _assetInsuranceCommandRepository.CreateAsync(assetInsurance);
            if (result.Id <= 0)
            {
                return new ApiResponseDTO<GetAssetInsuranceDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create Asset Insurance",
                    Data = null
                };
            }

            // Fetch newly created record
            var createdAssetInsurance = await _assetInsuranceQueryRepository.GetByAssetIdAsync(result.Id);
            var mappedResult = _mapper.Map<GetAssetInsuranceDto>(createdAssetInsurance);

            // Publish domain event for auditing/logging
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: mappedResult.AssetId.ToString(),
                actionName: mappedResult.Id.ToString(),
                details: $"Asset Insurance '{mappedResult.AssetId}' was created with PolicyNo: {mappedResult.PolicyNo}",
                module: "AssetInsurance"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            // Return success response
            return new ApiResponseDTO<GetAssetInsuranceDto>
            {
                IsSuccess = true,
                Message = "Asset Insurance created successfully",
                Data = mappedResult
            };
        }

      
    }
}